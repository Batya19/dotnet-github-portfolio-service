namespace GitHubPortfolio.Service.Models;

public class RepositoryInfo
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Url { get; set; } = string.Empty;
    public int Stars { get; set; }
    public string Languages { get; set; } = string.Empty;
    public DateTime? LastCommitDate { get; set; }
    public string? LastCommitMessage { get; set; }
    public string? LastCommitAuthor { get; set; }
    public int PullRequestCount { get; set; }
}