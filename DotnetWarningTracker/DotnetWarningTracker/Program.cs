using DotnetWarningTracker;

var walker = await GitBranchWalker
    .FromLastCommitsOfBranchAsync("main", 5);

await walker.ForeachAsync(async commit =>
{
    var nbWarnings = await WarningCounter.CountWarningsForCurrentDirectoryAsync();

    Console.WriteLine($"{commit}: {nbWarnings} warning(s)");
});
