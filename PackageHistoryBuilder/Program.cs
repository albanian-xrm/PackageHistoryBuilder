using CommandLine;
using System.Threading.Tasks;

public static class Program
{
    public static async Task Main(string[] args)
    {
        await Parser.Default
                    .ParseArguments<Options>(args)
                    .WithParsedAsync(
                        async (options) =>
                            await new PackageHistoryBuilder(options).Execute());
    }
}