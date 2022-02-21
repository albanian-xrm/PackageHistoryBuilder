using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.ProjectDecompiler;
using ICSharpCode.Decompiler.Metadata;
using System.Threading.Tasks;

internal class ILSpyClient
{
    public async Task DecompileAsync(string assemblyFileName, string targetDirectory)
    {
        using (var module = new PEFile(assemblyFileName))
        {
            var resolver = new UniversalAssemblyResolver(assemblyFileName, false, module.Reader.DetectTargetFrameworkId());
            var settings = new DecompilerSettings(LanguageVersion.CSharp7_2)
            {
                ThrowOnAssemblyResolveErrors = false,
                RemoveDeadCode = false,
                RemoveDeadStores = false,
                UseSdkStyleProjectFormat = WholeProjectDecompiler.CanUseSdkStyleProjectFormat(module),
            };
            var decompiler = new WholeProjectDecompiler(settings, resolver, resolver, null);
            await Task.Run(() => decompiler.DecompileProject(module, targetDirectory));
        }
    }
}
