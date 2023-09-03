using BlogRssFeed;
using Colors.Net;
using Microsoft.Extensions.Hosting;

Console.WriteLine("Welcome to Blogger Article writer from RSS feed!");
try
{

   await Host.CreateDefaultBuilder()
        .RunCommandLineApplicationAsync<CreateCommand>(args);
}
catch (Exception e)
{
    ColoredConsole.Error.WriteLine(e.Message);
}
