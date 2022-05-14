using System.Diagnostics.CodeAnalysis;

namespace DotnetWarningTracker.Command;

public record CommandOptions(
    string? GitBranch,
    int NbCommits
)
{
    [MemberNotNullWhen(true, nameof(GitBranch))]
    public bool GitBranchWalkingAsked => GitBranch is not null;
};