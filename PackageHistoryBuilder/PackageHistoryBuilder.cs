using LibGit2Sharp;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public class PackageHistoryBuilder
{
    private readonly NuGetClient nuGetClient = new NuGetClient();
    private readonly GitClient gitClient;
    private readonly ILSpyClient iLSpyClient = new ILSpyClient();
    private readonly Options options;

    public PackageHistoryBuilder(Options options)
    {
        this.options = options;
        gitClient = new GitClient(options.OutputPath);
    }

    public async Task Execute()
    {
        gitClient.Initialize();
        var versions = await nuGetClient.GetPackageVersions(options.PackageId);
        var tags = gitClient.GetRepositoryTags();
        foreach (var version in versions)
        {
            if (tags.Any((Tag tag) => tag.FriendlyName == version.ToString()))
            {
                Console.WriteLine($"{version} not found in Repository");
                continue;
            }
            string tempFolder = await nuGetClient.DownloadPackage(options.PackageId, version);
            var directory = new DirectoryInfo(tempFolder);
            var files = directory.GetFiles($"*{options.Assembly}", SearchOption.AllDirectories);
            if (files.Any())
            {
                try
                {
                    await iLSpyClient.DecompileAsync(files.First().FullName, options.OutputPath);
                    gitClient.CommitChanges(version.ToString(), version.ToString());
                }
                catch (BadImageFormatException) { }
            }
            try
            {
                directory.Delete(true);
            }
            catch (IOException) { }
        }
    }
}

