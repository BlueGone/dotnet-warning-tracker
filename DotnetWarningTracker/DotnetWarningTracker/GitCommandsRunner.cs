using CliWrap;
using CliWrap.Buffered;

namespace DotnetWarningTracker;

public static class GitCommandsRunner
{
    public static async Task<string[]> GetCommitSHAsAsync(string branch)
    {
        var gitLogResult = await Cli.Wrap("git")
            .WithArguments($"log --merges --first-parent {branch} --format=format:\"%h\"")
            .ExecuteBufferedAsync();

        return gitLogResult.StandardOutput.Split();
    }

    public static async Task<string> GetCommitMessageAsync(string gitReference)
    {
        var commandResult = await Cli.Wrap("git")
            .WithArguments($"show {gitReference} --no-patch --no-notes --pretty=%s")
            .ExecuteBufferedAsync();

        return commandResult.StandardOutput.Trim();
    }

    public static async Task<DateTime> GetCommitDateTimeAsync(string gitReference)
    {
        var commandResult = await Cli.Wrap("git")
            .WithArguments($"show {gitReference} --no-patch --no-notes --pretty=%ai")
            .ExecuteBufferedAsync();

        return DateTime.Parse(commandResult.StandardOutput);
    }

    public static async Task SwitchWithDetachedHeadAsync(string gitReference)
    {
        await Cli.Wrap("git")
            .WithArguments($"switch -d {gitReference}")
            .ExecuteBufferedAsync();
    }

    public static async Task<string> GetCurrentCommitShaAsync()
    {
        var gitShowResult = await Cli.Wrap("git")
            .WithArguments("show --format=format:%h")
            .ExecuteBufferedAsync();

        return gitShowResult.StandardOutput.TrimEnd();
    }
}