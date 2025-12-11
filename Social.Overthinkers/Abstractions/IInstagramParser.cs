namespace Social.Overthinkers.Abstractions;

public interface IInstagramParser
{
    DateTimeOffset ParseTimestamp(string? timeStamp);
}
