namespace SocialHub.Api.Configuration;

public sealed class MetaOptions
{
    public const string SectionName = "Meta";

    public string AppId { get; set; } = string.Empty;
    public string AppSecret { get; set; } = string.Empty;
    public string RedirectUri { get; set; } = string.Empty;
    public string GraphApiVersion { get; set; } = "v26.0";
    public string[] Scopes { get; set; } = Array.Empty<string>();
}
