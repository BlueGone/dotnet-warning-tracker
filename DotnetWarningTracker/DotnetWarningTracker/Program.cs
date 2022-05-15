using System.CommandLine;
using System.Globalization;
using DotnetWarningTracker.Command;
using DotnetWarningTracker.Reports;

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
        var report = options switch
        {
            { GitBranchWalkingAsked: true } =>
                options.UntilCommit is null
                    ? await HandleGitBranchWalkingCommandAsync(options.GitBranch, options.NbCommits)
                    : await HandleGitBranchWalkingCommandAsync(options.GitBranch, options.UntilCommit),
            _ => await HandleDefaultArgumentsCommandAsync()
        };

        var csvOutput = MakeCsvOutputFromReport(report);

        Console.WriteLine(csvOutput);
    }

    private static string MakeCsvOutputFromReport(IReport report)
    {
        using var writer = new StringWriter();
        using var csvWriter = new CsvHelper.CsvWriter(writer, CultureInfo.InvariantCulture);

        csvWriter.WriteRecords(report.GetCsvRecords());

        return writer.ToString();
    }

    private static async Task<IReport> HandleDefaultArgumentsCommandAsync()
    {
        return await WarningCounter.CountWarningsForCurrentDirectoryAsync();
    }

    private static async Task<IReport> HandleGitBranchWalkingCommandAsync(string gitBranch, int nbCommits)
    {
        var walker = await GitBranchWalker.FromLastCommitsOfBranchAsync(gitBranch, nbCommits);

        return await HandleGitBranchWalkingCommandAsync(gitBranch, walker);
    }

    private static async Task<IReport> HandleGitBranchWalkingCommandAsync(string gitBranch, string untilCommit)
    {
        var walker = await GitBranchWalker.UntilCommitOfBranchAsync(gitBranch, untilCommit);

        return await HandleGitBranchWalkingCommandAsync(gitBranch, walker);
    }

    private static async Task<IReport> HandleGitBranchWalkingCommandAsync(string gitBranch, GitBranchWalker walker)
    {
        var gitCommitReports = walker.MapAsync(async commitSha =>
        {
            var dotnetBuildReport = await WarningCounter.CountWarningsForCurrentDirectoryAsync();

            var commitMessage = await GitCommandsRunner.GetCommitMessageAsync(commitSha);
            var commitDateTime = await GitCommandsRunner.GetCommitDateTimeAsync(commitSha);

            return new GitCommitReport(dotnetBuildReport, commitSha, commitMessage, commitDateTime);
        });

        return new GitWalkingReport(await gitCommitReports.ToListAsync(), gitBranch);
    }
}
