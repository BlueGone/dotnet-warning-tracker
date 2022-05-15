namespace DotnetWarningTracker.Reports;

public record DotnetBuildReport(uint WarningsCount, QualifiedDotnetWarning[] QualifiedDotnetWarnings) : IReport
{
    public IEnumerable<object> GetCsvRecords() => new[]
    {
        new { WarningsCount }
    };
}