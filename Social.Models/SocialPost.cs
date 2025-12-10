using Social.Models.Enums;

namespace Social.Models;

public record SocialPost(
    SocialMedia Platform,
    string Id,
    string UrlToShare,
    string Title,
    DateTime DetectedAt);
