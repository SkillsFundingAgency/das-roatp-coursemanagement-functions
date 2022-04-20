﻿using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.Roatp.CourseManagement.Jobs;

[assembly: FunctionsStartup(typeof(Startup))]
namespace SFA.DAS.Roatp.CourseManagement.Jobs
{
    internal class Startup : FunctionsStartup
    {
        private IConfiguration _configuration;
        public override void Configure(IFunctionsHostBuilder builder)
        {
            _configuration = BuildConfiguration(builder);

            AddNLog(builder);
        }
        private static void AddNLog(IFunctionsHostBuilder builder)
        {
            builder.Services.AddLogging((options) =>
            {
                options.AddFilter(typeof(Startup).Namespace, LogLevel.Information);
                options.SetMinimumLevel(LogLevel.Trace);
                options.AddNLog(new NLogProviderOptions
                {
                    CaptureMessageTemplates = true,
                    CaptureMessageProperties = true
                });
                options.AddConsole();

                NLogConfiguration.ConfigureNLog();
            });
        }

        private static IConfiguration BuildConfiguration(IFunctionsHostBuilder builder)
        {
            var configuration = builder.GetContext().Configuration;
            var configBuilder = new ConfigurationBuilder()
                .AddConfiguration(configuration);

            var env = configuration["EnvironmentName"];
            if (env != "LOCAL")
            {
                configBuilder.AddAzureTableStorage(options =>
                {
                    options.ConfigurationKeys = new[] { "SFA.DAS.Roatp.CourseManagement.Jobs" };
                    options.StorageConnectionString = configuration["ConfigurationStorageConnectionString"];
                    options.EnvironmentName = configuration["EnvironmentName"];
                    options.PreFixConfigurationKeys = false;
                });
            }

            var config = configBuilder.Build();
            builder.Services.Replace(ServiceDescriptor.Singleton(typeof(IConfiguration), config));
            return config;
        }
    }
}
