namespace DotnetWarningTracker.Reports;

public record GitWalkingReport(
    List<GitCommitReport> GitCommitReports,
    string BranchName
) : IReport
{
    public IEnumerable<object> GetCsvRecords() => GitCommitReports
        .Select(gitCommitReport => new
        {
            gitCommitReport.CommitMessage,
            gitCommitReport.CommitSha,
            CommitDate = DateOnly.FromDateTime(gitCommitReport.CommitDateTime),
            CommitTime = TimeOnly.FromDateTime(gitCommitReport.CommitDateTime),
            gitCommitReport.DotnetBuildReport.WarningsCount,
        });
}