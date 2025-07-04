using GitHubPortfolio.Service.Cache;
using GitHubPortfolio.Service.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Octokit;
using System.Text;

namespace GitHubPortfolio.Service.Services;

public class GitHubService : IGitHubService
{
    private readonly ICacheService _cacheService;
    private readonly ILogger<GitHubService> _logger;
    private readonly GitHubClient _gitHubClient;
    private readonly string _username;

    public GitHubService(ICacheService cacheService,
                         IOptions<GitHubPortfolioOptions> options,
                         ILogger<GitHubService> logger)
    {
        _cacheService = cacheService;
        _logger = logger;
        _username = options.Value.Username;

        // Create GitHub client with token authentication
        _gitHubClient = new GitHubClient(new ProductHeaderValue("GitHubPortfolio"));
        _gitHubClient.Credentials = new Credentials(options.Value.Token);
    }

    public async Task<IEnumerable<RepositoryInfo>> GetPortfolioAsync(CancellationToken cancellationToken = default)
    {
        // Try to get repositories from cache
        string cacheKey = $"portfolio_{_username}";
        if (_cacheService.TryGetValue(cacheKey, out IEnumerable<RepositoryInfo>? repositories))
        {
            _logger.LogInformation("Retrieved portfolio from cache for user {Username}", _username);
            return repositories!;
        }

        try
        {
            // Get repositories from GitHub API
            var userRepositories = await _gitHubClient.Repository.GetAllForUser(_username);

            // Process repositories in parallel
            var result = await Task.WhenAll(
                userRepositories.Select(async repo =>
                {
                    try
                    {
                        // Get last commit
                        var commitInfo = await GetLastCommitAsync(repo.Owner.Login, repo.Name);

                        // Get Pull Request count
                        var pullRequestCount = await GetPullRequestCountAsync(repo.Owner.Login, repo.Name);

                        // Get languages
                        var languagesDict = await _gitHubClient.Repository.GetAllLanguages(repo.Owner.Login, repo.Name);
                        var languages = string.Join(", ", languagesDict.Select(l => l.Name));

                        return new RepositoryInfo
                        {
                            Id = repo.Id,
                            Name = repo.Name,
                            Description = repo.Description,
                            Url = repo.HtmlUrl,
                            Stars = repo.StargazersCount,
                            Languages = languages,
                            LastCommitDate = commitInfo.Date,
                            LastCommitMessage = commitInfo.Message,
                            LastCommitAuthor = commitInfo.Author,
                            PullRequestCount = pullRequestCount
                        };
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error getting details for repository {RepoName}", repo.Name);
                        return new RepositoryInfo
                        {
                            Id = repo.Id,
                            Name = repo.Name,
                            Description = repo.Description,
                            Url = repo.HtmlUrl,
                            Stars = repo.StargazersCount,
                            Languages = "Unknown",
                            LastCommitDate = null,
                            LastCommitMessage = "Error retrieving commit information",
                            LastCommitAuthor = "Unknown",
                            PullRequestCount = 0
                        };
                    }
                }));

            // Save to cache
            _cacheService.Set(cacheKey, result, TimeSpan.FromMinutes(30));

            _logger.LogInformation("Retrieved portfolio from GitHub API for user {Username}", _username);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving portfolio for user {Username}", _username);
            throw;
        }
    }

    public async Task<IEnumerable<RepositoryInfo>> SearchRepositoriesAsync(SearchOptions options, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(options.RepositoryName) &&
            string.IsNullOrWhiteSpace(options.Language) &&
            string.IsNullOrWhiteSpace(options.Username))
        {
            return Array.Empty<RepositoryInfo>();
        }

        // Build search query
        var searchBuilder = new StringBuilder();

        // Add repository name if provided
        if (!string.IsNullOrWhiteSpace(options.RepositoryName))
        {
            searchBuilder.Append(options.RepositoryName).Append(' ');
        }

        // Add language if provided
        if (!string.IsNullOrWhiteSpace(options.Language))
        {
            searchBuilder.Append("language:").Append(options.Language).Append(' ');
        }

        // Add username if provided
        if (!string.IsNullOrWhiteSpace(options.Username))
        {
            searchBuilder.Append("user:").Append(options.Username);
        }

        var searchRequest = new SearchRepositoriesRequest(searchBuilder.ToString())
        {
            SortField = RepoSearchSort.Stars,
            Order = SortDirection.Descending,
            PerPage = 20
        };

        try
        {
            var searchResult = await _gitHubClient.Search.SearchRepo(searchRequest);

            // Process repositories
            var result = await Task.WhenAll(
                searchResult.Items.Select(async repo =>
                {
                    try
                    {
                        // Get last commit
                        var commitInfo = await GetLastCommitAsync(repo.Owner.Login, repo.Name);

                        // Get Pull Request count
                        var pullRequestCount = await GetPullRequestCountAsync(repo.Owner.Login, repo.Name);

                        // Get languages
                        var languagesDict = await _gitHubClient.Repository.GetAllLanguages(repo.Owner.Login, repo.Name);
                        var languages = string.Join(", ", languagesDict.Select(l => l.Name));

                        return new RepositoryInfo
                        {
                            Id = repo.Id,
                            Name = repo.Name,
                            Description = repo.Description,
                            Url = repo.HtmlUrl,
                            Stars = repo.StargazersCount,
                            Languages = languages,
                            LastCommitDate = commitInfo.Date,
                            LastCommitMessage = commitInfo.Message,
                            LastCommitAuthor = commitInfo.Author,
                            PullRequestCount = pullRequestCount
                        };
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error getting details for repository {RepoName}", repo.Name);
                        return new RepositoryInfo
                        {
                            Id = repo.Id,
                            Name = repo.Name,
                            Description = repo.Description,
                            Url = repo.HtmlUrl,
                            Stars = repo.StargazersCount,
                            Languages = "Unknown",
                            LastCommitDate = null,
                            LastCommitMessage = "Error retrieving commit information",
                            LastCommitAuthor = "Unknown",
                            PullRequestCount = 0
                        };
                    }
                }));

            _logger.LogInformation("Search returned {Count} repositories", result.Length);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching repositories");
            throw;
        }
    }

    public async Task<bool> HasUserActivitySinceAsync(DateTime lastCheck, CancellationToken cancellationToken = default)
    {
        try
        {
            // Get user events from GitHub API
            var events = await _gitHubClient.Activity.Events.GetAllUserPerformed(_username);

            // Check if any event occurred after the last check
            return events.Any(e => e.CreatedAt > lastCheck);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking user activity for {Username}", _username);
            return false;
        }
    }

    private async Task<(DateTime? Date, string? Message, string? Author)> GetLastCommitAsync(string owner, string repoName)
    {
        try
        {
            // Use options that work with the current Octokit version
            var commitRequest = new CommitRequest
            {
                // Adjusting the request to use 'Since' to limit the number of commits retrieved
                Since = DateTimeOffset.UtcNow.AddYears(-1) // Example: Retrieve commits from the last year
            };

            var commits = await _gitHubClient.Repository.Commit.GetAll(owner, repoName, commitRequest);

            if (commits.Count == 0)
            {
                return (null, null, null);
            }

            var commit = commits[0];
            return (commit.Commit.Author.Date.UtcDateTime, commit.Commit.Message, commit.Commit.Author.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving last commit for repository {RepoName}", repoName);
            return (null, null, null);
        }
    }

    private async Task<int> GetPullRequestCountAsync(string owner, string repoName)
    {
        try
        {
            var pullRequests = await _gitHubClient.PullRequest.GetAllForRepository(owner, repoName);
            return pullRequests.Count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving pull requests for repository {RepoName}", repoName);
            return 0;
        }
    }
}