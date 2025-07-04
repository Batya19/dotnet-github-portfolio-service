using System.Text.Json.Serialization;

namespace GitHubPortfolio.Service.Models;

public class GitHubPortfolioOptions
{
    public const string SectionName = "GitHubPortfolio";

    public string Username { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}