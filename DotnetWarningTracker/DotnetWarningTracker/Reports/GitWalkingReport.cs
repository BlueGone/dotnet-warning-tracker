namespace DotnetWarningTracker.Reports;

public record GitWalkingReport(
    List<GitCommitReport> GitCommitReports,
    string BranchName
) : IReport
{
    public IEnumerable<object> GetCsvRecords() => GitCommitReports
        .Select(gitCommitReport => new
        {
            gitCommitReport.CommitSha,
            gitCommitReport.DotnetBuildReport.WarningsCount
        });
}