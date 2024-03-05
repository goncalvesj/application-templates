using Octokit;

namespace AzDO.GH.Function;

public interface IGitHubClientService
{
    Task<Issue> CreateGitHubIssue(string owner, string repo, NewIssue newIssue);
}

public class GitHubClientService : IGitHubClientService
{
    private readonly string _ghPat;
    private readonly string _productHeaderValue;
    private readonly GitHubClient _client;

    public GitHubClientService()
    {
        _ghPat = Environment.GetEnvironmentVariable("GITHUB_PAT") ?? throw new ArgumentNullException(nameof(_ghPat));
        _productHeaderValue = Environment.GetEnvironmentVariable("PRODUCT_HEADER_VALUE") ?? throw new ArgumentNullException(nameof(_productHeaderValue));
        var credentials = new Credentials(_ghPat);
        _client = new GitHubClient(new ProductHeaderValue(_productHeaderValue)) { Credentials = credentials };
    }

    public async Task<Issue> CreateGitHubIssue(string owner, string repo, NewIssue newIssue)
    {
        var issue = await _client.Issue.Create(owner, repo, newIssue);
        return issue;
    }
}