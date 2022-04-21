using CliWrap;
using CliWrap.Buffered;

namespace DotnetWarningTracker;

class GitCheckoutContext : IAsyncDisposable
{
    private readonly string _currentCommit;

    private GitCheckoutContext(string currentCommit)
    {
        _currentCommit = currentCommit;
    }

    public static async Task<GitCheckoutContext> AcquireAsync(string gitReference)
    {
        var gitShowResult = await Cli.Wrap("git")
            .WithArguments("show --format=format:%h")
            .ExecuteBufferedAsync();

        var currentCommit = gitShowResult.StandardOutput.TrimEnd();

        await Cli.Wrap("git")
            .WithArguments($"switch -d {gitReference}")
            .ExecuteBufferedAsync();

        return new GitCheckoutContext(currentCommit);
    }

    public async ValueTask DisposeAsync()
    {
        await Cli.Wrap("git")
            .WithArguments($"switch -d {_currentCommit}")
            .ExecuteBufferedAsync();
    }
}