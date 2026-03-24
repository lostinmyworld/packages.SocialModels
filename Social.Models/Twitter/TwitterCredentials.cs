namespace Social.Models.Twitter;

public record TwitterCredentials(
    string ConsumerKey,
    string ConsumerSecret,
    string AccessToken,
    string AccessSecret);
