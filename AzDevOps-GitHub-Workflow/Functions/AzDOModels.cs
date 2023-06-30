using System.Text.Json.Serialization;

namespace AzDO.GH.Function.Models;

public record Account(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("baseUrl")] string BaseUrl
);

public record Attributes(
    [property: JsonPropertyName("isLocked")] bool IsLocked,
    [property: JsonPropertyName("name")] string Name
);

public record Collection(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("baseUrl")] string BaseUrl
);

public record DetailedMessage(
    [property: JsonPropertyName("text")] string Text,
    [property: JsonPropertyName("html")] string Html,
    [property: JsonPropertyName("markdown")] string Markdown
);

public record Fields(
    [property: JsonPropertyName("System.AreaPath")] string SystemAreaPath,
    [property: JsonPropertyName("System.TeamProject")] string SystemTeamProject,
    [property: JsonPropertyName("System.IterationPath")] string SystemIterationPath,
    [property: JsonPropertyName("System.WorkItemType")] string SystemWorkItemType,
    [property: JsonPropertyName("System.State")] string SystemState,
    [property: JsonPropertyName("System.Reason")] string SystemReason,
    [property: JsonPropertyName("System.CreatedDate")] DateTime SystemCreatedDate,
    [property: JsonPropertyName("System.CreatedBy")] string SystemCreatedBy,
    [property: JsonPropertyName("System.ChangedDate")] DateTime SystemChangedDate,
    [property: JsonPropertyName("System.ChangedBy")] string SystemChangedBy,
    [property: JsonPropertyName("System.CommentCount")] int SystemCommentCount,
    [property: JsonPropertyName("System.Title")] string SystemTitle,
    [property: JsonPropertyName("Microsoft.VSTS.Common.StateChangeDate")] DateTime MicrosoftVSTSCommonStateChangeDate,
    [property: JsonPropertyName("Microsoft.VSTS.Common.Priority")] int MicrosoftVSTSCommonPriority,
    [property: JsonPropertyName("System.Tags")] string SystemTags,
    [property: JsonPropertyName("System.Parent")] int SystemParent,
    [property: JsonPropertyName("href")] string Href
);

public record Html(
    [property: JsonPropertyName("href")] string Href
);

public record Links(
    [property: JsonPropertyName("self")] Self Self,
    [property: JsonPropertyName("workItemUpdates")] WorkItemUpdates WorkItemUpdates,
    [property: JsonPropertyName("workItemRevisions")] WorkItemRevisions WorkItemRevisions,
    [property: JsonPropertyName("workItemComments")] WorkItemComments WorkItemComments,
    [property: JsonPropertyName("html")] Html Html,
    [property: JsonPropertyName("workItemType")] WorkItemType WorkItemType,
    [property: JsonPropertyName("fields")] Fields Fields
);

public record Message(
    [property: JsonPropertyName("text")] string Text,
    [property: JsonPropertyName("html")] string Html,
    [property: JsonPropertyName("markdown")] string Markdown
);

public record Project(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("baseUrl")] string BaseUrl
);

public record Relation(
    [property: JsonPropertyName("rel")] string Rel,
    [property: JsonPropertyName("url")] string Url,
    [property: JsonPropertyName("attributes")] Attributes Attributes
);

public record Resource(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("rev")] int Rev,
    [property: JsonPropertyName("fields")] Fields Fields,
    [property: JsonPropertyName("relations")] IReadOnlyList<Relation> Relations,
    [property: JsonPropertyName("_links")] Links Links,
    [property: JsonPropertyName("url")] string Url
);

public record ResourceContainers(
    [property: JsonPropertyName("collection")] Collection Collection,
    [property: JsonPropertyName("account")] Account Account,
    [property: JsonPropertyName("project")] Project Project
);

public record AzDORoot(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("eventType")] string EventType,
    [property: JsonPropertyName("publisherId")] string PublisherId,
    [property: JsonPropertyName("message")] Message Message,
    [property: JsonPropertyName("detailedMessage")] DetailedMessage DetailedMessage,
    [property: JsonPropertyName("resource")] Resource Resource,
    [property: JsonPropertyName("resourceVersion")] string ResourceVersion,
    [property: JsonPropertyName("resourceContainers")] ResourceContainers ResourceContainers,
    [property: JsonPropertyName("createdDate")] DateTime CreatedDate
);

public record Self(
    [property: JsonPropertyName("href")] string Href
);

public record WorkItemComments(
    [property: JsonPropertyName("href")] string Href
);

public record WorkItemRevisions(
    [property: JsonPropertyName("href")] string Href
);

public record WorkItemType(
    [property: JsonPropertyName("href")] string Href
);

public record WorkItemUpdates(
    [property: JsonPropertyName("href")] string Href
);