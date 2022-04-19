using DotnetWarningTracker;

var warningCount = await WarningCounter.CountWarningsForCurrentDirectoryAsync();
Console.WriteLine(warningCount);