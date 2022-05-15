namespace DotnetWarningTracker.Reports;

public record QualifiedDotnetWarning(
    string SourceFilePath,
    uint Line,
    uint Column,
    string RuleId,
    string Message,
    string CsprojPath
);