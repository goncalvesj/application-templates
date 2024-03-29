using AzDO.GH.Function.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.RegularExpressions;

namespace AzDO.GH.Function;

public class CloseIssue
{
    private readonly ILogger _logger;
    private readonly IAzDOClientService _azdoClient;
    private static readonly Regex pattern = new(@"\[(AB#)(\d+)\]", RegexOptions.Compiled, TimeSpan.FromMilliseconds(100));

    public CloseIssue(ILoggerFactory loggerFactory, IAzDOClientService azdoClient)
    {
        _logger = loggerFactory.CreateLogger<CloseIssue>();
        _azdoClient = azdoClient;
    }

    [Function("closetaskfromissue")]
    public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
    {
        _logger.LogInformation("HTTP trigger started");

        var content = await req.ReadFromJsonAsync<GitHubRoot>();

        if (content != null && content.Issue.State == "closed")
        {
            var issue = content.Issue.Body;

            //var pattern = @"\[(AB#)(\d+)\]";
            //var matches = Regex.Matches(issue, pattern, RegexOptions.None, TimeSpan.FromMilliseconds(100));

            var matches = pattern.Matches(issue);

            for (int i = 0; i < matches.Count; i++)
            {
                Match match = matches[i];
                var workItem = await _azdoClient.UpdateWorkItemStatus(Convert.ToInt32(match.Groups[2].Value));
                _logger.LogInformation("Data: {workItem}", workItem.Url);
            }

            var successResponse = req.CreateResponse(HttpStatusCode.OK);
            await successResponse.WriteAsJsonAsync(new { message = "Tasks closed successfully" });
            return successResponse;
        }

        var message = "Issue not found or status is not closed";
        _logger.LogInformation("Message: {message}", message);

        var response = req.CreateResponse(HttpStatusCode.BadRequest);
        await response.WriteAsJsonAsync(new { error = message });
        return response;
    }
}