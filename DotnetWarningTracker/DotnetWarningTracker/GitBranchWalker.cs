namespace DotnetWarningTracker;

public class GitBranchWalker
{
    private readonly IReadOnlyCollection<string> _commitSHAs;

    private GitBranchWalker(IReadOnlyCollection<string> commitShAs)
    {
        _commitSHAs = commitShAs;
    }

    public static async Task<GitBranchWalker> FromLastCommitsOfBranchAsync(string branch, int commitsLimit)
    {
        var commitSHAs = await GitCommandsRunner.GetCommitSHAsAsync(branch);

        return new GitBranchWalker(commitSHAs.Take(commitsLimit).Reverse().ToList());
    }

    public async IAsyncEnumerable<TOutput> MapAsync<TOutput>(Func<string, Task<TOutput>> action)
    {
        foreach (var commit in _commitSHAs)
        {
            await using var _ = await GitCheckoutContext.AcquireAsync(commit);

            yield return await action(commit);
        }
    }
}