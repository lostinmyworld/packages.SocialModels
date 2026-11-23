namespace Social.Models.Threads;

public record ThreadsPostRequest(
    string AccessToken,
    string Text,
    string? LinkUrl = null);
