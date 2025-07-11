using GitHubPortfolio.Service.Models;
using GitHubPortfolio.Service.Services;
using Microsoft.AspNetCore.Mvc;

namespace GitHubPortfolio.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GitHubController : ControllerBase
{
    private readonly IGitHubService _gitHubService;
    private readonly ILogger<GitHubController> _logger;

    public GitHubController(
        IGitHubService gitHubService,
        ILogger<GitHubController> logger)
    {
        _gitHubService = gitHubService;
        _logger = logger;
    }

    /// <summary>
    /// Gets the user's portfolio from GitHub
    /// </summary>
    /// <returns>A list of repository information</returns>
    [HttpGet("portfolio")]
    public async Task<ActionResult<IEnumerable<RepositoryInfo>>> GetPortfolio()
    {
        try
        {
            var result = await _gitHubService.GetPortfolioAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting portfolio");
            return StatusCode(500, "An error occurred while getting the portfolio");
        }
    }

    /// <summary>
    /// Searches for repositories on GitHub
    /// </summary>
    /// <param name="repositoryName">Repository name to search for</param>
    /// <param name="language">Programming language to filter by</param>
    /// <param name="username">GitHub username to filter by</param>
    /// <param name="page">Page number</param>
    /// <param name="perPage">Items per page</param>
    /// <returns>A list of repository information matching the search criteria</returns>
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<RepositoryInfo>>> SearchRepositories(
        [FromQuery] string? repositoryName = null,
        [FromQuery] string? language = null,
        [FromQuery] string? username = null,
        [FromQuery] int page = 1,
        [FromQuery] int perPage = 30)
    {
        try
        {
            var options = new SearchOptions
            {
                RepositoryName = repositoryName,
                Language = language,
                Username = username,
            };

            var result = await _gitHubService.SearchRepositoriesAsync(options);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching repositories");
            return StatusCode(500, "An error occurred while searching repositories");
        }
    }
}