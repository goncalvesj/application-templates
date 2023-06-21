using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Octokit;

namespace AzDO.GH.Function;

public class CreateIssue
{
    private readonly ILogger _logger;
    private readonly GitHubClient _githubClient;

    public CreateIssue(ILoggerFactory loggerFactory, GitHubClient githubClient)
    {
        _logger = loggerFactory.CreateLogger<CreateIssue>();
        _githubClient = githubClient;
    }

    [Function("createissuefromtask")]
    public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
    {
        _logger.LogInformation("HTTP trigger started");

        try
        {
            var content = await req.ReadFromJsonAsync<Root>();

            var systemTags = content?.resource?.fields?.SystemTags;
            if (string.IsNullOrEmpty(systemTags))
            {
                var response = req.CreateResponse(HttpStatusCode.BadRequest);
                await response.WriteAsJsonAsync(new { error = "SystemTags field not found" });
                return response;
            }

            var tags = systemTags.Split(';');
            var owner = tags.FirstOrDefault(t => t.Trim().StartsWith("owner:", StringComparison.OrdinalIgnoreCase))?.Split(':')[1]?.Trim();
            var repo = tags.FirstOrDefault(t => t.Trim().StartsWith("repo:", StringComparison.OrdinalIgnoreCase))?.Split(':')[1]?.Trim();

            if (string.IsNullOrEmpty(owner) || string.IsNullOrEmpty(repo))
            {
                var response = req.CreateResponse(HttpStatusCode.BadRequest);
                await response.WriteAsJsonAsync(new { error = "Invalid owner or repo tag" });
                return response;
            }

            var newIssue = new NewIssue(content?.resource.fields.SystemTitle)
            {
                Body = $"Azure Boards WorkItem: AB#{content?.resource.id}"
            };

            var issue = await _githubClient.Issue.Create(owner, repo, newIssue);

            var successResponse = req.CreateResponse(HttpStatusCode.OK);
            await successResponse.WriteAsJsonAsync(new
            {
                issue.Id,
                issue.Number,
                issue.Title,
                issue.HtmlUrl
            });
            return successResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating issue");
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteAsJsonAsync(new { error = "Error creating issue" });
            return errorResponse;
        }
    }
}
