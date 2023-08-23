using AzDO.GH.Function.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Octokit;
using System.Net;

namespace AzDO.GH.Function;

public class CreateIssue
{
    private readonly ILogger _logger;
    private readonly IGitHubClientService _githubClient;

    public CreateIssue(ILoggerFactory loggerFactory, IGitHubClientService githubClient)
    {
        _logger = loggerFactory.CreateLogger<CreateIssue>();
        _githubClient = githubClient;
    }

    [Function("createissuefromtask")]
    public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
    {
        _logger.LogInformation("HTTP trigger started");

        var content = await req.ReadFromJsonAsync<AzDORoot>();

        var systemTags = content?.Resource?.Fields?.SystemTags;
        if (string.IsNullOrEmpty(systemTags))
        {
            var message = "SystemTags field not found";
            _logger.LogInformation("Message: {message}", message);

            var response = req.CreateResponse(HttpStatusCode.BadRequest);
            await response.WriteAsJsonAsync(new { error = message });
            return response;
        }

        var tags = systemTags.Split(';');
        var owner = tags.FirstOrDefault(t => t.Trim().StartsWith("owner:", StringComparison.OrdinalIgnoreCase))?.Split(':')[1]?.Trim();
        var repo = tags.FirstOrDefault(t => t.Trim().StartsWith("repo:", StringComparison.OrdinalIgnoreCase))?.Split(':')[1]?.Trim();

        if (string.IsNullOrEmpty(owner) || string.IsNullOrEmpty(repo))
        {
            var message = "Invalid owner or repo tag";
            _logger.LogInformation("Message: {message}", message);

            var response = req.CreateResponse(HttpStatusCode.BadRequest);
            await response.WriteAsJsonAsync(new { error = message });
            return response;
        }

        var newIssue = new NewIssue(content?.Resource.Fields.SystemTitle)
        {
            Body = $"Azure Boards WorkItem: AB#{content?.Resource.Id}"
        };

        var issue = await _githubClient.CreateGitHubIssue(owner, repo, newIssue);

        var successResponse = req.CreateResponse(HttpStatusCode.OK);
        await successResponse.WriteAsJsonAsync(new
        {
            issue.Id,
            issue.Number,
            issue.Title,
            issue.HtmlUrl
        });

        _logger.LogInformation("GH Issue created successfully");
        return successResponse;
    }
}
