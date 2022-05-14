namespace DotnetWarningTracker.Reports;

public record GitCommitReport(
    DotnetBuildReport DotnetBuildReport,
    string CommitSha
) : IReport
{
    public string ToReportString() => $"{CommitSha}: {DotnetBuildReport.WarningsCount} warning(s)";
}