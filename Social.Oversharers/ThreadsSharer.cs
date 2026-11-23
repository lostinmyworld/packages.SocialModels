using System.Text.Json;
using Social.Models;
using Social.Models.Threads;
using Social.Oversharers.Abstractions;

namespace Social.Oversharers;

public class ThreadsSharer : IThreadsSharer
{
    private const int DefaultPublishDelaySeconds = 30;
    private const string TextMediaType = "TEXT";
    private const string UnknownError = "unknown error";
    private const string CreateStep = "create";
    private const string PublishStep = "publish";

    private static readonly string[] _errorFieldNames =
    [
        "message",
        "error_user_title",
        "error_user_msg",
        "code",
        "error_subcode",
    ];

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly TimeSpan _publishDelay;

    public ThreadsSharer(IHttpClientFactory httpClientFactory, int publishDelaySeconds = DefaultPublishDelaySeconds)
    {
        ArgumentNullException.ThrowIfNull(httpClientFactory);

        _httpClientFactory = httpClientFactory;
        _publishDelay = TimeSpan.FromSeconds(publishDelaySeconds);
    }

    public async Task<ThreadsPostResponse> SharePost(ThreadsPostRequest request)
    {
        Console.WriteLine("Publishing post to Threads...");

        if (string.IsNullOrWhiteSpace(request?.Text))
        {
            return new(false, ErrorMessage: "Threads text cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(request.AccessToken))
        {
            return new(false, ErrorMessage: "Threads access token is missing.");
        }

        try
        {
            using var httpClient = _httpClientFactory.CreateClient(nameof(ThreadsSharer));

            var containerId = await CreateContainer(httpClient, request);

            await Task.Delay(_publishDelay);

            var postId = await PublishContainer(httpClient, containerId, request.AccessToken);

            Console.WriteLine($"Successfully posted to Threads. Post ID: {postId}");

            return new(true, PostId: postId);
        }
        catch (Exception ex)
        {
            await Console.Error.WriteLineAsync($"Failed to post to Threads: {ex.Message}.");
            return new(false, ErrorMessage: ex.Message);
        }
    }

    private static async Task<string> CreateContainer(HttpClient httpClient, ThreadsPostRequest request)
    {
        var formFields = new Dictionary<string, string>
        {
            ["media_type"] = TextMediaType,
            ["text"] = request.Text,
            ["access_token"] = request.AccessToken,
        };

        if (!string.IsNullOrWhiteSpace(request.LinkUrl))
        {
            formFields["link_attachment"] = request.LinkUrl;
        }

        return await PostForm(httpClient, CreateStep, Endpoints.ThreadsCreateContainerUri, formFields);
    }

    private static async Task<string> PublishContainer(HttpClient httpClient, string containerId, string accessToken)
    {
        var formFields = new Dictionary<string, string>
        {
            ["creation_id"] = containerId,
            ["access_token"] = accessToken,
        };

        return await PostForm(httpClient, PublishStep, Endpoints.ThreadsPublishUri, formFields);
    }

    private static async Task<string> PostForm(
        HttpClient httpClient,
        string step,
        string uri,
        Dictionary<string, string> formFields)
    {
        using var content = new FormUrlEncodedContent(formFields);
        using var response = await httpClient.PostAsync(uri, content);

        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            var errorDetails = ReadErrorDetails(responseBody);
            throw new HttpRequestException($"Threads API ({step}) returned {(int)response.StatusCode}: {errorDetails}");
        }

        var id = ReadId(responseBody);

        if (string.IsNullOrWhiteSpace(id))
        {
            throw new InvalidOperationException($"Threads API ({step}) returned no id.");
        }

        return id;
    }

    private static string? ReadId(string responseBody)
    {
        try
        {
            using var document = JsonDocument.Parse(responseBody);

            var hasId = document.RootElement.TryGetProperty("id", out var idElement);

            return hasId ? idElement.GetString() : null;
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private static string ReadErrorDetails(string responseBody)
    {
        try
        {
            using var document = JsonDocument.Parse(responseBody);

            var hasError = document.RootElement.TryGetProperty("error", out var errorElement);

            if (!hasError || errorElement.ValueKind != JsonValueKind.Object)
            {
                return UnknownError;
            }

            var details = new List<string>();

            foreach (var fieldName in _errorFieldNames)
            {
                var fieldValue = ReadFieldAsText(errorElement, fieldName);

                if (!string.IsNullOrWhiteSpace(fieldValue))
                {
                    details.Add($"{fieldName}: {fieldValue}");
                }
            }

            return details.Count == 0
                ? UnknownError
                : string.Join(" | ", details);
        }
        catch (JsonException)
        {
            return UnknownError;
        }
    }

    private static string? ReadFieldAsText(JsonElement element, string fieldName)
    {
        if (!element.TryGetProperty(fieldName, out var fieldElement))
        {
            return null;
        }

        return fieldElement.ValueKind switch
        {
            JsonValueKind.String => fieldElement.GetString(),
            JsonValueKind.Number => fieldElement.GetRawText(),
            _ => null,
        };
    }
}
