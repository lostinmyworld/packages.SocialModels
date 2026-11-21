using Social.Models.Enums;
using Social.Models.Gist;
using Social.Oversharers;

namespace Social.Tests;

public class EnvironmentLoaderTests
{
    private readonly EnvironmentLoader _loader = new();

    [Fact]
    public void LoadPerSocial_VariableSet_ReturnsItsValue()
    {
        var variableName = SetUniqueVariable("webhook-value");

        var result = _loader.LoadPerSocial(new() { { SocialMedia.YouTube, variableName } });

        Assert.Equal("webhook-value", result[SocialMedia.YouTube]);
    }

    [Fact]
    public void LoadPerSocial_VariableMissing_LeavesSocialOut()
    {
        var result = _loader.LoadPerSocial(new() { { SocialMedia.YouTube, UniqueVariableName() } });

        Assert.Empty(result);
    }

    [Fact]
    public void LoadGistOptions_VariablesSet_ReturnsTheirValues()
    {
        var options = new GistOptions
        {
            GistId = SetUniqueVariable("id-value"),
            GistToken = SetUniqueVariable("token-value"),
            GistStateFileName = SetUniqueVariable("file-value"),
        };

        var result = _loader.LoadGistOptions(options);

        Assert.Equal("id-value", result?.GistId);
        Assert.Equal("token-value", result?.GistToken);
        Assert.Equal("file-value", result?.GistStateFileName);
    }

    [Fact]
    public void LoadGistOptions_VariablesMissing_ReturnsEmptyValues()
    {
        var options = new GistOptions
        {
            GistId = UniqueVariableName(),
            GistToken = UniqueVariableName(),
            GistStateFileName = UniqueVariableName(),
        };

        var result = _loader.LoadGistOptions(options);

        Assert.Equal(string.Empty, result?.GistId);
        Assert.Equal(string.Empty, result?.GistToken);
        Assert.Equal(string.Empty, result?.GistStateFileName);
    }

    private static string UniqueVariableName()
        => $"SOCIAL_TESTS_{Guid.NewGuid():N}";

    private static string SetUniqueVariable(string value)
    {
        var name = UniqueVariableName();
        Environment.SetEnvironmentVariable(name, value);

        return name;
    }
}
