using Microsoft.Extensions.DependencyInjection;
using Social.Models.Gist;
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

Console.WriteLine("Test success!");