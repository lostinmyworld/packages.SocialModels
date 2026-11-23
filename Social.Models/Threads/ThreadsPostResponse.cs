namespace Social.Models.Threads;

public record ThreadsPostResponse(
    bool IsSuccess,
    string? PostId = null,
    string? ErrorMessage = null);
