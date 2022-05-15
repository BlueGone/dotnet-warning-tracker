namespace DotnetWarningTracker.Reports;

public record GitCommitReport(
    DotnetBuildReport DotnetBuildReport,

    string CommitSha,
    string CommitMessage,
    DateTime CommitDateTime
) : IReport
{
    public IEnumerable<object> GetCsvRecords() => new[]
    {
        new
        {
            CommitSha,
            CommitMessage,
            Date = DateOnly.FromDateTime(CommitDateTime),
            Time = TimeOnly.FromDateTime(CommitDateTime),
            DotnetBuildReport.WarningsCount
        }
    };
}