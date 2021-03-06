﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace SwarmApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(config => {
                    var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                    var cwd = Directory.GetCurrentDirectory();
                    
                    config.AddJsonFile(Path.Combine(cwd, "appsettings.json"), false);
                    config.AddJsonFile(Path.Combine(cwd, $"appsettings.{environment}.json"), true);
                })
                .UseStartup<Startup>()
                .UseUrls("http://0.0.0.0:5050")
                .Build();
    }
}
