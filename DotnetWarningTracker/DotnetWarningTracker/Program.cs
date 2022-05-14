using System.CommandLine;
using DotnetWarningTracker.Command;

namespace DotnetWarningTracker;

static class Program
{
    public static async Task<int> Main(string[] args)
    {
        var rootCommand = CommandBuilder.BuildRootCommand(HandleCommandAsync);

        return await rootCommand.InvokeAsync(args);
    }

    static async Task HandleCommandAsync(CommandOptions options)
    {
        if (options.GitBranchWalkingAsked)
        {
            await HandleGitBranchWalkingCommandAsync(options.GitBranch, options.NbCommits);
        }
        else
        {
            await HandleDefaultArgumentsCommandAsync();
        }
    }

    private static async Task HandleDefaultArgumentsCommandAsync()
    {
        var nbWarnings = await WarningCounter.CountWarningsForCurrentDirectoryAsync();

        Console.WriteLine($"{nbWarnings} warning(s)");
    }

    private static async Task HandleGitBranchWalkingCommandAsync(string gitBranch, int nbCommits)
    {
        var walker = await GitBranchWalker.FromLastCommitsOfBranchAsync(gitBranch, nbCommits);

        await walker.ForeachAsync(async commit =>
        {
            var nbWarnings = await WarningCounter.CountWarningsForCurrentDirectoryAsync();

            Console.WriteLine($"{commit}: {nbWarnings} warning(s)");
        });
    }
}
