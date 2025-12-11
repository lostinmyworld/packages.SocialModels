using System.Globalization;
using Social.Overthinkers.Abstractions;

namespace Social.Overthinkers;

public class InstagramParser : IInstagramParser
{
    public DateTimeOffset ParseTimestamp(string? timeStamp)
    {
        if (string.IsNullOrWhiteSpace(timeStamp))
        {
            return DateTimeOffset.MinValue;
        }

        if (timeStamp.EndsWith("+0000"))
        {
            timeStamp = timeStamp[..^5] + "+00:00";
        }

        if (DateTimeOffset.TryParse(timeStamp, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var dto))
        {
            return dto;
        }

        return DateTimeOffset.MinValue;
    }
}
