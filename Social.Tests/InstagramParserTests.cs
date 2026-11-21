using Social.Overthinkers;

namespace Social.Tests;

public class InstagramParserTests
{
    private readonly InstagramParser _parser = new();

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void ParseTimestamp_NoTimestamp_ReturnsMinValue(string? timestamp)
    {
        var result = _parser.ParseTimestamp(timestamp);

        Assert.Equal(DateTimeOffset.MinValue, result);
    }

    [Fact]
    public void ParseTimestamp_InstagramFormatWithoutColon_ReturnsUtcTime()
    {
        var result = _parser.ParseTimestamp("2026-03-01T10:15:30+0000");

        Assert.Equal(new DateTimeOffset(2026, 3, 1, 10, 15, 30, TimeSpan.Zero), result);
    }

    [Fact]
    public void ParseTimestamp_IsoFormatWithOffset_ReturnsTimeWithOffset()
    {
        var result = _parser.ParseTimestamp("2026-03-01T10:15:30+02:00");

        Assert.Equal(new DateTimeOffset(2026, 3, 1, 10, 15, 30, TimeSpan.FromHours(2)), result);
    }

    [Fact]
    public void ParseTimestamp_InvalidText_ReturnsMinValue()
    {
        var result = _parser.ParseTimestamp("not a date");

        Assert.Equal(DateTimeOffset.MinValue, result);
    }
}
