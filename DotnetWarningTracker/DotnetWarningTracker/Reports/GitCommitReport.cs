namespace DotnetWarningTracker.Reports;

public record GitCommitReport(
    DotnetBuildReport DotnetBuildReport,
    string CommitSha
) : IReport
{
    public IEnumerable<object> GetCsvRecords() => new[] { this };
}