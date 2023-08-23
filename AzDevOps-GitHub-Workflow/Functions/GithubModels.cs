using System.Text.Json.Serialization;

namespace AzDO.GH.Function.Models;
public record Issue(
    [property: JsonPropertyName("url")] string Url,
    [property: JsonPropertyName("repository_url")] string RepositoryUrl,
    [property: JsonPropertyName("labels_url")] string LabelsUrl,
    [property: JsonPropertyName("comments_url")] string CommentsUrl,
    [property: JsonPropertyName("events_url")] string EventsUrl,
    [property: JsonPropertyName("html_url")] string HtmlUrl,
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("node_id")] string NodeId,
    [property: JsonPropertyName("number")] int Number,
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("labels")] IReadOnlyList<object> Labels,
    [property: JsonPropertyName("state")] string State,
    [property: JsonPropertyName("locked")] bool Locked,
    [property: JsonPropertyName("assignee")] object Assignee,
    [property: JsonPropertyName("assignees")] IReadOnlyList<object> Assignees,
    [property: JsonPropertyName("milestone")] object Milestone,
    [property: JsonPropertyName("comments")] int Comments,
    [property: JsonPropertyName("created_at")] DateTime CreatedAt,
    [property: JsonPropertyName("updated_at")] DateTime UpdatedAt,
    [property: JsonPropertyName("closed_at")] DateTime ClosedAt,
    [property: JsonPropertyName("author_association")] string AuthorAssociation,
    [property: JsonPropertyName("active_lock_reason")] object ActiveLockReason,
    [property: JsonPropertyName("body")] string Body,
    [property: JsonPropertyName("timeline_url")] string TimelineUrl,
    [property: JsonPropertyName("performed_via_github_app")] object PerformedViaGithubApp,
    [property: JsonPropertyName("state_reason")] string StateReason
);

public record GitHubRoot(
    [property: JsonPropertyName("action")] string Action,
    [property: JsonPropertyName("issue")] Issue Issue
);