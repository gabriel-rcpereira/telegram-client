using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.WindowsServices;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace TLC.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string[] parameters = GetParameters(args);

            var isService = !(Debugger.IsAttached || args.Contains("--console"));
            if (isService)
            {
                SetCurrentDirectoryRunningAsService();
                CreateWebHostBuilder(parameters)
                    .Build()
                    .RunAsService();
            }
            else
            {
                CreateWebHostBuilder(parameters)
                    .Build()
                    .Run();
            }
        }

        private static void SetCurrentDirectoryRunningAsService()
        {
            var pathToExe = Process.GetCurrentProcess().MainModule.FileName;
            var pathToContentRoot = Path.GetDirectoryName(pathToExe);
            Directory.SetCurrentDirectory(pathToContentRoot);
        }

        private static string[] GetParameters(string[] args)
        {
            return args.Where(arg => arg != "--console").ToArray();
        }

        private static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
