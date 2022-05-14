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

        return new DotnetBuildReport(warningsCount);
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
}