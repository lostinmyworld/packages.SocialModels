using System.Text.Json;
using Social.Models;
using Social.Models.Twitter;
using Social.Oversharers.Abstractions;
using Tweetinvi;

namespace Social.Oversharers;

public class TwitterSharer : ITwitterSharer
{
    public async Task<TwitterPostResponse> SharePost(
        TwitterCredentials credentials,
        TwitterPostRequest request)
    {
        Console.WriteLine("Publishing post to X (Twitter)...");

        if (string.IsNullOrWhiteSpace(request.Text))
        {
            return new(false, ErrorMessage: "Tweet text cannot be empty.");
        }

        try
        {
            var twitterCredentials = new Tweetinvi.Models.TwitterCredentials(
                credentials.ConsumerKey,
                credentials.ConsumerSecret,
                credentials.AccessToken,
                credentials.AccessSecret);

            var twitterClient = new TwitterClient(twitterCredentials);
            var jsonBody = JsonSerializer.Serialize(request.Text);

            var response = await twitterClient.Execute.AdvanceRequestAsync(twitterRequest =>
            {
                twitterRequest.Query.Url = Endpoints.TwitterV2QueryUrl;
                twitterRequest.Query.HttpMethod = Tweetinvi.Models.HttpMethod.POST;
                twitterRequest.Query.HttpContent = new StringContent(jsonBody);
            });

            var doc = JsonDocument.Parse(response.Content);
            var tweetId = doc.RootElement.GetProperty("data")
                .GetProperty("id")
                .GetString();

            Console.WriteLine($"Successfully posted to X. Tweet ID: {tweetId}");

            return new(true, TweetId: tweetId);
        }
        catch (Exception ex)
        {
            await Console.Error.WriteLineAsync($"Failed to post to X: {ex.Message}.");
            return new(false, ErrorMessage: ex.Message);
        }
    }
}
