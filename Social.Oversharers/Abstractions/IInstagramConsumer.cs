using Social.Models.Instagram;

namespace Social.Oversharers.Abstractions;

public interface IInstagramConsumer
{
    Task<List<InstagramPost>> RetrievePostsAsync(InstagramRequest request);
}
