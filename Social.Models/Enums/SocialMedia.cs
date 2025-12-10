using System.Text.Json.Serialization;

namespace Social.Models.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SocialMedia
{
    Discord,
    X,
    Facebook,
    Instagram,
    TikTok,
    YouTube,
}
