using OpsRelay.Api.Extensions;

namespace OpsRelay.Api;

public class Program
{
    public static void Main(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseOpsRelaySerilog()
            .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>())
            .Build()
            .Run();
}
