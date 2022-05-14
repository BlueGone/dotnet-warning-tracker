namespace DotnetWarningTracker.Reports;

public record GitWalkingReport(
    List<GitCommitReport> GitCommitReports,
    string BranchName
) : IReport
{
    public IEnumerable<object> GetCsvRecords() => GitCommitReports;
}