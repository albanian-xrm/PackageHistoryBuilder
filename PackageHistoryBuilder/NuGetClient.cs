using NuGet.Common;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

internal class NuGetClient
{
    private readonly SourceCacheContext cache;
    private readonly PackageDownloadContext downloadContext;
    private readonly CancellationToken cancellationToken = CancellationToken.None;
    private readonly ILogger logger = NullLogger.Instance;
    private readonly SourceRepository repository;

    public NuGetClient(string source = "https://api.nuget.org/v3/index.json")
    {
        repository = Repository.Factory.GetCoreV3(source);
        cache = new SourceCacheContext();
        downloadContext = new PackageDownloadContext(cache);
    }

    public async Task<IEnumerable<NuGetVersion>> GetPackageVersions(string packageId)
    {
        FindPackageByIdResource resource = await repository.GetResourceAsync<FindPackageByIdResource>();
        return await resource.GetAllVersionsAsync(
            packageId,
            cache,
            logger,
            cancellationToken);
    }

    internal async Task<string> DownloadPackage(string packageId, NuGetVersion version)
    {
        DownloadResource resource = await repository.GetResourceAsync<DownloadResource>();
        await resource.GetDownloadResourceResultAsync(
            new NuGet.Packaging.Core.PackageIdentity(packageId, version),
             downloadContext,
             Path.GetTempPath(),
             logger,
             cancellationToken);
        return Path.Combine(Path.GetTempPath(), packageId, version.ToString());
    }
}
