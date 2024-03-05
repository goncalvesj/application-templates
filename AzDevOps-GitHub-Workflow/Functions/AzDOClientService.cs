using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;

namespace AzDO.GH.Function;

public interface IAzDOClientService
{
    Task<WorkItem> UpdateWorkItemStatus(int workItemId);
}

public class AzDOClientService : IAzDOClientService
{
    private readonly string _pat;
    private readonly string _org;
    private readonly WorkItemTrackingHttpClient _witClient;

    public AzDOClientService()
    {
        _pat = Environment.GetEnvironmentVariable("AZUREDEVOPS_PAT") ??
        throw new ArgumentNullException(nameof(_pat));
        _org = Environment.GetEnvironmentVariable("AZUREDEVOPS_ORG") ??
        throw new ArgumentNullException(nameof(_org));

        var connection = new VssConnection(new Uri(_org), new VssBasicCredential(string.Empty, _pat));
        _witClient = connection.GetClient<WorkItemTrackingHttpClient>();
    }

    public async Task<WorkItem> UpdateWorkItemStatus(int workItemId)
    {
        var patchDocument = new JsonPatchDocument
        {
            new JsonPatchOperation
            {
                Operation = Operation.Add,
                Path = "/fields/System.State",
                Value = "Closed"
            }
        };

        return await _witClient.UpdateWorkItemAsync(patchDocument, workItemId);
    }
}