using System.Net;

namespace Social.Tests;

public class SequenceHttpMessageHandler(params (HttpStatusCode StatusCode, string Body)[] responses) : HttpMessageHandler
{
    private int _responseIndex;

    public List<HttpRequestMessage> Requests { get; } = [];

    public List<string> RequestBodies { get; } = [];

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Requests.Add(request);

        var requestBody = request.Content is null
            ? string.Empty
            : await request.Content.ReadAsStringAsync(cancellationToken);
        RequestBodies.Add(requestBody);

        var (statusCode, body) = responses[_responseIndex];
        _responseIndex++;

        return new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(body),
        };
    }
}
