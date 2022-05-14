namespace DotnetWarningTracker.Reports;

public interface IReport
{
    IEnumerable<object> GetCsvRecords();
}