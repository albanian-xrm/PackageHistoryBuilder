using CommandLine;

public class Options
{
    [Option('a', Required = false, HelpText = "Assembly to decompile")]
    public string Assembly { get; set; }

    [Option('o', Required = true, HelpText = "Output path")]
    public string OutputPath { get; set; }

    [Option('p', Required = true, HelpText = "The NuGet package Id")]
    public string PackageId { get; set; }
}
