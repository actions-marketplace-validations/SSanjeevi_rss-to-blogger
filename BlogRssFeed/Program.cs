using BlogRssFeed;
using Colors.Net;
using Microsoft.Extensions.CommandLineUtils;

Console.WriteLine("Welcome to Blogger Article writer from RSS feed!");
try
{
    var app = new CommandLineApplication()
    {
        Name = "BloggerCLI",
        FullName = "BloggerCLI",
        Description = "BloggerCLI"
    };

    app.Commands.Add(new CreateCommand());

    app.OnExecute(() =>
    {
        ColoredConsole.Error.WriteLine("No commands specified, please specify a command");
        app.ShowHelp();
        return 1;
    });
    return app.Execute(args);
}
catch (Exception e)
{
    ColoredConsole.Error.WriteLine(e.Message);
    return 1;
}
