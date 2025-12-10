using Social.Models.Enums;
using Social.Models.Gist;

namespace Social.Oversharers.Abstractions;

public interface IEnvironmentLoader
{
    Dictionary<SocialMedia, string> LoadPerSocial(Dictionary<SocialMedia, string>? environmentVariables);
    GistOptions? LoadGistOptions(GistOptions options);
    Dictionary<SocialMedia, GistOptions>? LoadGistOptionsPerSocial(Dictionary<SocialMedia, GistOptions>? gistOptionsPerSocial);
}