using System.Runtime.InteropServices;

namespace CapstoneBackend.Core;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        //we should really only ever be using dev or prod, but might as well do it right.
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        
        var builder = Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
                webBuilder.UseStartup<Startup>())
            .ConfigureAppConfiguration(configuration =>
            {
                configuration.SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false)
                    .AddJsonFile($"appsettings.{env}.json", optional: false)
                    .AddEnvironmentVariables()
                    .Build();
            });
            
        return builder;
    }
}