using Social.Models.Threads;

namespace Social.Oversharers.Abstractions;

public interface IThreadsSharer
{
    Task<ThreadsPostResponse> SharePost(ThreadsPostRequest request);
}
