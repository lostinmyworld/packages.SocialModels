using Social.Models.Twitter;

namespace Social.Oversharers.Abstractions;

public interface ITwitterSharer
{
    Task<TwitterPostResponse> SharePost(TwitterCredentials credentials, TwitterPostRequest request);
}
