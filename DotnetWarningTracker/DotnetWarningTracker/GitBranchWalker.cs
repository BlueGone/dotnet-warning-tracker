using CliWrap;
using CliWrap.Buffered;

namespace DotnetWarningTracker;

public class GitBranchWalker
{
    const int DefaultCommitsLimit = 10;

    private readonly IReadOnlyCollection<string> _commits;

    private GitBranchWalker(IReadOnlyCollection<string> commits)
    {
        _commits = commits;
    }

    public static async Task<GitBranchWalker> FromLastCommitsOfBranchAsync(string branch, int commitsLimit = DefaultCommitsLimit)
    {
        var gitLogResult = await Cli.Wrap("git")
            .WithArguments($"log --merges --first-parent {branch} --format=format:\"%h\"")
            .ExecuteBufferedAsync();

        var commits = gitLogResult.StandardOutput.Split().Take(commitsLimit).Reverse().ToList();

        return new GitBranchWalker(commits);
    }

    public async IAsyncEnumerable<TOutput> MapAsync<TOutput>(Func<string, Task<TOutput>> action)
    {
        foreach (var commit in _commits)
        {
            await using var _ = await GitCheckoutContext.AcquireAsync(commit);

            yield return await action(commit);
        }
    }
}