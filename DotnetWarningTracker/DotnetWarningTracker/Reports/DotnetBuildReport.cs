namespace DotnetWarningTracker.Reports;

public record DotnetBuildReport(uint WarningsCount) : IReport
{
    public IEnumerable<object> GetCsvRecords() => new[] { this };
}