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

    public static async Task<GitBranchWalker> UntilCommitOfBranchAsync(string branch, string targetCommitSha)
    {
        var commitSHAs = await GitCommandsRunner.GetCommitSHAsAsync(branch);

        var commitsUntilTarget = commitSHAs.Reverse().SkipWhile(commitSha => commitSha != targetCommitSha).ToList();

        if (!commitsUntilTarget.Any())
        {
            throw new ApplicationException($"Could not find commit {targetCommitSha} in branch {branch}");
        }

        return new GitBranchWalker(commitsUntilTarget);
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