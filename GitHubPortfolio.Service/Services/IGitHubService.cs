using GitHubPortfolio.Service.Models;

namespace GitHubPortfolio.Service.Services;

public interface IGitHubService
{
    Task<IEnumerable<RepositoryInfo>> GetPortfolioAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<RepositoryInfo>> SearchRepositoriesAsync(SearchOptions options, CancellationToken cancellationToken = default);
    Task<bool> HasUserActivitySinceAsync(DateTime lastCheck, CancellationToken cancellationToken = default);
}