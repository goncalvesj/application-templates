using System.Text.Json.Serialization;

namespace AzDO.GH.Function;

public class Account
{
    [JsonPropertyName("id")]
    public string id { get; set; }

    [JsonPropertyName("baseUrl")]
    public string baseUrl { get; set; }
}

public class Attributes
{
    [JsonPropertyName("isLocked")]
    public bool isLocked { get; set; }

    [JsonPropertyName("name")]
    public string name { get; set; }
}

public class Collection
{
    [JsonPropertyName("id")]
    public string id { get; set; }

    [JsonPropertyName("baseUrl")]
    public string baseUrl { get; set; }
}

public class DetailedMessage
{
    [JsonPropertyName("text")]
    public string text { get; set; }

    [JsonPropertyName("html")]
    public string html { get; set; }

    [JsonPropertyName("markdown")]
    public string markdown { get; set; }
}

public class Fields
{
    [JsonPropertyName("System.AreaPath")]
    public string SystemAreaPath { get; set; }

    [JsonPropertyName("System.TeamProject")]
    public string SystemTeamProject { get; set; }

    [JsonPropertyName("System.IterationPath")]
    public string SystemIterationPath { get; set; }

    [JsonPropertyName("System.WorkItemType")]
    public string SystemWorkItemType { get; set; }

    [JsonPropertyName("System.State")]
    public string SystemState { get; set; }

    [JsonPropertyName("System.Reason")]
    public string SystemReason { get; set; }

    [JsonPropertyName("System.CreatedDate")]
    public DateTime SystemCreatedDate { get; set; }

    [JsonPropertyName("System.CreatedBy")]
    public string SystemCreatedBy { get; set; }

    [JsonPropertyName("System.ChangedDate")]
    public DateTime SystemChangedDate { get; set; }

    [JsonPropertyName("System.ChangedBy")]
    public string SystemChangedBy { get; set; }

    [JsonPropertyName("System.CommentCount")]
    public int SystemCommentCount { get; set; }

    [JsonPropertyName("System.Title")]
    public string SystemTitle { get; set; }

    [JsonPropertyName("Microsoft.VSTS.Common.StateChangeDate")]
    public DateTime MicrosoftVSTSCommonStateChangeDate { get; set; }

    [JsonPropertyName("Microsoft.VSTS.Common.Priority")]
    public int MicrosoftVSTSCommonPriority { get; set; }

    [JsonPropertyName("System.Tags")]
    public string SystemTags { get; set; }

    [JsonPropertyName("System.Parent")]
    public int SystemParent { get; set; }

    [JsonPropertyName("href")]
    public string href { get; set; }
}

public class Html
{
    [JsonPropertyName("href")]
    public string href { get; set; }
}

public class Links
{
    [JsonPropertyName("self")]
    public Self self { get; set; }

    [JsonPropertyName("workItemUpdates")]
    public WorkItemUpdates workItemUpdates { get; set; }

    [JsonPropertyName("workItemRevisions")]
    public WorkItemRevisions workItemRevisions { get; set; }

    [JsonPropertyName("workItemComments")]
    public WorkItemComments workItemComments { get; set; }

    [JsonPropertyName("html")]
    public Html html { get; set; }

    [JsonPropertyName("workItemType")]
    public WorkItemType workItemType { get; set; }

    [JsonPropertyName("fields")]
    public Fields fields { get; set; }
}

public class Message
{
    [JsonPropertyName("text")]
    public string text { get; set; }

    [JsonPropertyName("html")]
    public string html { get; set; }

    [JsonPropertyName("markdown")]
    public string markdown { get; set; }
}

public class Project
{
    [JsonPropertyName("id")]
    public string id { get; set; }

    [JsonPropertyName("baseUrl")]
    public string baseUrl { get; set; }
}

public class Relation
{
    [JsonPropertyName("rel")]
    public string rel { get; set; }

    [JsonPropertyName("url")]
    public string url { get; set; }

    [JsonPropertyName("attributes")]
    public Attributes attributes { get; set; }
}

public class Resource
{
    [JsonPropertyName("id")]
    public int id { get; set; }

    [JsonPropertyName("rev")]
    public int rev { get; set; }

    [JsonPropertyName("fields")]
    public Fields fields { get; set; }

    [JsonPropertyName("relations")]
    public List<Relation> relations { get; set; }

    [JsonPropertyName("_links")]
    public Links _links { get; set; }

    [JsonPropertyName("url")]
    public string url { get; set; }
}

public class ResourceContainers
{
    [JsonPropertyName("collection")]
    public Collection collection { get; set; }

    [JsonPropertyName("account")]
    public Account account { get; set; }

    [JsonPropertyName("project")]
    public Project project { get; set; }
}

public class Root
{
    [JsonPropertyName("id")]
    public string id { get; set; }

    [JsonPropertyName("eventType")]
    public string eventType { get; set; }

    [JsonPropertyName("publisherId")]
    public string publisherId { get; set; }

    [JsonPropertyName("message")]
    public Message message { get; set; }

    [JsonPropertyName("detailedMessage")]
    public DetailedMessage detailedMessage { get; set; }

    [JsonPropertyName("resource")]
    public Resource resource { get; set; }

    [JsonPropertyName("resourceVersion")]
    public string resourceVersion { get; set; }

    [JsonPropertyName("resourceContainers")]
    public ResourceContainers resourceContainers { get; set; }

    [JsonPropertyName("createdDate")]
    public DateTime createdDate { get; set; }
}

public class Self
{
    [JsonPropertyName("href")]
    public string href { get; set; }
}

public class WorkItemComments
{
    [JsonPropertyName("href")]
    public string href { get; set; }
}

public class WorkItemRevisions
{
    [JsonPropertyName("href")]
    public string href { get; set; }
}

public class WorkItemType
{
    [JsonPropertyName("href")]
    public string href { get; set; }
}

public class WorkItemUpdates
{
    [JsonPropertyName("href")]
    public string href { get; set; }
}
