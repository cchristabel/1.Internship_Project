using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SecureMVCApp.Services;

namespace SecureMVCApp
{
    public class Program
    {
        public static void Main(string[] args)
    {
        // create the host for the application
        var host = CreateHostBuilder(args).Build();

        // check if a CSV file path is provided via command-line arguments
        if (args.Length > 0)
        {
            var filePath = args[0];  // first argument is the file path
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    // resolve the CSV import service
                    var csvUserImportService = services.GetRequiredService<CsvUserImportService>();

                    // import users from the provided CSV file
                    csvUserImportService.ImportUsersFromCsv(filePath);
                    Console.WriteLine("Users imported successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error importing users: {ex.Message}");
                }
            }
        }
        else
        {
            Console.WriteLine("No CSV file path provided.");
        }

        // run the web host if no command-line argument is provided
        host.Run();
    }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile("users.json", optional: false, reloadOnChange: true);
                })

                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
