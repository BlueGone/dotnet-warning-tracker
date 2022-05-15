using System.Text.RegularExpressions;
using CliWrap;
using CliWrap.Buffered;
using DotnetWarningTracker.Reports;

namespace DotnetWarningTracker;

public static class WarningCounter
{
    public static async Task<DotnetBuildReport> CountWarningsForCurrentDirectoryAsync()
    {
        var result = await Cli.Wrap("dotnet")
            .WithArguments("build --no-incremental")
            .WithEnvironmentVariables(new Dictionary<string, string?>
            {
                ["DOTNET_CLI_UI_LANGUAGE"] = "en",
            })
            .ExecuteBufferedAsync();

        var warningsCount = GetWarningCountFromOutput(result.StandardOutput);

        var qualifiedDotnetWarnings = GetQualifiedDotnetWarningsFromOutput(result.StandardOutput);

        return new DotnetBuildReport(warningsCount, qualifiedDotnetWarnings);
    }

    private static uint GetWarningCountFromOutput(string processOutput)
    {
        var warningsCountRegex = new Regex("(?<warning_count>\\d+) Warning\\(s\\)", RegexOptions.Compiled);

        var regexMatch = warningsCountRegex.Match(processOutput);
        if (!regexMatch.Success)
        {
            throw new InvalidProgramException("Could not find warnings count in process output");
        }

        return uint.Parse(regexMatch.Groups["warning_count"].Value);
    }

    private static QualifiedDotnetWarning[] GetQualifiedDotnetWarningsFromOutput(string processOutput)
    {
        return processOutput
            .Split(Environment.NewLine)
            .Select(GetQualifiedDotnetWarningFromLine)
            .OfType<QualifiedDotnetWarning>()
            .Distinct()
            .ToArray();
    }

    private static QualifiedDotnetWarning? GetQualifiedDotnetWarningFromLine(string line)
    {
        var regex = new Regex(
            "^(?<source_file_path>[^(]+)\\((?<line>\\d+),(?<column>\\d+)\\): warning (?<rule_id>\\w*): (?<message>.*) \\[(?<csproj_path>.*)\\]",
            RegexOptions.Compiled);

        var match = regex.Match(line);

        if (!match.Success)
        {
            return null;
        }

        return new QualifiedDotnetWarning(
            match.Groups["source_file_path"].Value,
            uint.Parse(match.Groups["line"].Value),
            uint.Parse(match.Groups["column"].Value),
            match.Groups["rule_id"].Value,
            match.Groups["message"].Value,
            match.Groups["csproj_path"].Value
        );
    }
}