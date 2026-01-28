using Microsoft.Extensions.DependencyInjection;
using Social.Models.Gist;
using Social.Models.Instagram;
using Social.Oversharers.Abstractions;
using Social.Oversharers.Extensions;
using Social.Overthinkers.Extensions;

Console.WriteLine("Adding dependencies...");

var services = new ServiceCollection();

services.AddSocialOverThinkers();
services.AddSocialOverSharers();

var serviceProvider = services.BuildServiceProvider();

Console.WriteLine("Getting dependencies...");

var environmentLoader = serviceProvider.GetRequiredService<IEnvironmentLoader>();
var gistConsumer = serviceProvider.GetRequiredService<IGistConsumer>();
var instaConsumer = serviceProvider.GetRequiredService<IInstagramConsumer>();

Console.WriteLine("Getting envitonment variables...");

var gistOptionsToRetrieve = new GistOptions()
{
    GistId = "GIST_ID",
    GistToken = "GIST_TOKEN",
    GistStateFileName = "GIST_STATE_FILE_NAME",
};
var gistOptions = environmentLoader.LoadGistOptions(gistOptionsToRetrieve);

Console.WriteLine("Getting Gist State...");

var state = await gistConsumer.LoadPreviousState(gistOptions!, "test");

var instagramToken = Environment.GetEnvironmentVariable("IG_ACCESS_TOKEN");

var instaRequest = new InstagramRequest(instagramToken, "insta-watchdog/1.0", HowManyPostsToFetch: 20);

var instaData = await instaConsumer.RetrievePostsAsync(instaRequest);

Console.WriteLine("Test success!");