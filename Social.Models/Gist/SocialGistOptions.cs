using Social.Models.Enums;

namespace Social.Models.Gist;

public class SocialGistOptions
{
    public Dictionary<SocialMedia, GistOptions> GistPerSocial { get; set; } = [];
}
