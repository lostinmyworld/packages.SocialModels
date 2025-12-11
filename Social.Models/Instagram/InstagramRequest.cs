namespace Social.Models.Instagram;

public record InstagramRequest(
    string Token,
    string App,
    int HowManyPostsToFetch = 20,
    string InstagramFieldsToRetrieve = "id,caption,media_type,media_url,permalink,timestamp");
