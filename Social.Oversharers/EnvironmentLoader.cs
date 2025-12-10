using Social.Models.Enums;
using Social.Models.Gist;
using Social.Oversharers.Abstractions;

namespace Social.Oversharers;

public class EnvironmentLoader : IEnvironmentLoader
{
    public EnvironmentLoader()
    {
        SetEnvironmentVariablesFromLocalFile(".env.local");
    }

    public Dictionary<SocialMedia, string> LoadPerSocial(Dictionary<SocialMedia, string>? environmentVariables)
    {
        if (environmentVariables is null
            || environmentVariables.Count == 0)
        {
            Console.WriteLine($"Could not load environment variables. Method: {nameof(LoadPerSocial)}.");

            return [];
        }

        var options = new Dictionary<SocialMedia, string>();

        foreach (var kv in environmentVariables)
        {
            var value = Environment.GetEnvironmentVariable(kv.Value);
            if (!string.IsNullOrWhiteSpace(value))
            {
                options.Add(kv.Key, value);
            }
        }

        return options;
    }

    public GistOptions? LoadGistOptions(GistOptions? options)
    {
        if (options is null)
        {
            return null;
        }

        return new()
        {
            GistId = Environment.GetEnvironmentVariable(options.GistId)
                ?? string.Empty,
            GistStateFileName = Environment.GetEnvironmentVariable(options.GistStateFileName)
                ?? string.Empty,
            GistToken = Environment.GetEnvironmentVariable(options.GistToken)
                ?? string.Empty,
        };
    }

    public Dictionary<SocialMedia, GistOptions>? LoadGistOptionsPerSocial(Dictionary<SocialMedia, GistOptions>? gistOptionsPerSocial)
    {
        if (gistOptionsPerSocial is null
            || gistOptionsPerSocial.Count == 0)
        {
            Console.WriteLine($"Could not load environment variables. Method: {nameof(LoadGistOptionsPerSocial)}.");

            return [];
        }

        var options = new Dictionary<SocialMedia, GistOptions>();

        foreach (var kv in gistOptionsPerSocial)
        {
            var gistOptions = LoadGistOptions(kv.Value);
            if (gistOptions is not null)
            {
                options.Add(kv.Key, gistOptions);
            }
        }

        return options;
    }

    private static void SetEnvironmentVariablesFromLocalFile(string path)
    {
        try
        {
            if (!File.Exists(path))
            {
                return;
            }

            Console.WriteLine($"Loading local env from {path}...");
            var lines = File.ReadAllLines(path);

            foreach (var raw in lines)
            {
                var line = raw.Trim();
                if (string.IsNullOrWhiteSpace(line)
                    || line.StartsWith('#'))
                {
                    continue;
                }

                var index = line.IndexOf('=', StringComparison.Ordinal);
                if (index <= 0)
                {
                    continue;
                }

                var key = line[..index].Trim();
                var value = line[(index + 1)..].Trim();

                Environment.SetEnvironmentVariable(key, value);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to load .env.local: " + ex.Message);
        }
    }
}
