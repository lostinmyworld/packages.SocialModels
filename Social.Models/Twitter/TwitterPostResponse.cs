namespace Social.Models.Twitter;

public record TwitterPostResponse(
    bool IsSuccess,
    string? TweetId = null,
    string? ErrorMessage = null);
