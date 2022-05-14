using System.CommandLine;
using System.CommandLine.NamingConventionBinder;

namespace DotnetWarningTracker.Command;

public static class CommandBuilder
{
    private const string CommandDescription = "A command-line utility to keep track of your dotnet warnings";

    public static RootCommand BuildRootCommand(Func<CommandOptions, Task> handler)
    {
        var command = new RootCommand(CommandDescription)
        {
            Handler = CommandHandler.Create(handler)
        };

        command.AddOption(new Option<string>("--git-branch"));
        command.AddOption(new Option<int>("--nb-commits", getDefaultValue: () => 10));

        return command;
    }
}