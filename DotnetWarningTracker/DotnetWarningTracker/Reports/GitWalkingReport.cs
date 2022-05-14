using System.Text;

namespace DotnetWarningTracker.Reports;

public record GitWalkingReport(
    List<GitCommitReport> GitCommitReports,
    string BranchName
) : IReport
{
    public string ToReportString()
    {
        var stringBuilder = new StringBuilder($"{BranchName}:").AppendLine();

        GitCommitReports.ForEach(gitCommitReport =>
        {
            stringBuilder
                .Append('\t')
                .Append(gitCommitReport.ToReportString())
                .AppendLine();
        });

        return stringBuilder.ToString();
    }
}