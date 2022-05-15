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
        var currentCommit = await GitCommandsRunner.GetCurrentCommitShaAsync();
        await GitCommandsRunner.SwitchWithDetachedHeadAsync(gitReference);

        return new GitCheckoutContext(currentCommit);
    }

    public async ValueTask DisposeAsync()
    {
        await GitCommandsRunner.SwitchWithDetachedHeadAsync(_currentCommit);
    }
}