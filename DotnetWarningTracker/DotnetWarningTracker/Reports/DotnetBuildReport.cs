namespace DotnetWarningTracker.Reports;

public record DotnetBuildReport(uint WarningsCount) : IReport
{
    public string ToReportString() => $"{WarningsCount} warning(s)";
}