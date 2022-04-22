using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NLog.Extensions.Logging;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.Roatp.CourseManagement.Jobs;
using SFA.DAS.Roatp.CourseManagement.Jobs.Functions;
using SFA.DAS.Roatp.CourseManagement.Jobs.Infrastructure.ApiClients;
using SFA.DAS.Roatp.CourseManagement.Jobs.Infrastructure.Tokens;


[assembly: FunctionsStartup(typeof(Startup))]
namespace SFA.DAS.Roatp.CourseManagement.Jobs
{
    internal class Startup : FunctionsStartup
    {
        private IConfiguration _configuration;
        public override void Configure(IFunctionsHostBuilder builder)
        {
            _configuration = BuildConfiguration(builder);
            BuildHttpClients(builder);
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
            builder.Services.AddOptions(); //?
            // MFCMFC
            var coursesConfig = config.GetSection("CoursesApiConfiguration");
            var roatpV2Config = config.GetSection("RoatpV2ApiConfiguration");
            builder.Services.Configure<CoursesApiConfiguration>(coursesConfig);
            builder.Services.Configure<RoatpV2ApiConfiguration>(roatpV2Config);
            return config;
        }

        private static void BuildHttpClients(IFunctionsHostBuilder builder)
        {
            var acceptHeaderName = "Accept";
            var acceptHeaderValue = "application/json";
            var handlerLifeTime = TimeSpan.FromMinutes(5);
        
            builder.Services.AddHttpClient<IGetAllCoursesApiClient, CoursesGetAllApiClient>((serviceProvider, httpClient) =>
                {
                    var coursesApiConfiguration = serviceProvider.GetService<IOptions<CoursesApiConfiguration>>().Value;
                   
                 
                    // MFCMFC get coursesApiConfiguration working
                      if (string.IsNullOrEmpty(coursesApiConfiguration.Url))
                          coursesApiConfiguration.Url = "https://localhost:5001/";

                  ////////
            
                    httpClient.BaseAddress = new Uri(coursesApiConfiguration.Url);
                 
                    
                    httpClient.DefaultRequestHeaders.Add(acceptHeaderName, acceptHeaderValue);
                    
                    httpClient.DefaultRequestHeaders.Add("X-Version","1"); 
            
                    var configuration = serviceProvider.GetService<IConfiguration>();
                    if (!configuration["EnvironmentName"].Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase))
                    {
                        var generateTokenTask = BearerTokenGenerator.GenerateTokenAsync(coursesApiConfiguration.Identifier);
                        httpClient.DefaultRequestHeaders.Authorization = generateTokenTask.GetAwaiter().GetResult();
                    }
                })
                .SetHandlerLifetime(handlerLifeTime);


            builder.Services.AddHttpClient<IRoatpV2UpdateCourseDetailsApiClient, RoatpV2UpdateCourseDetailsApiClient>((serviceProvider, httpClient) =>
                {
                     var roatpV2ApiConfiguration = serviceProvider.GetService<IOptions<RoatpV2ApiConfiguration>>().Value;

                    // MFCMFC get roatpV2ApiConfiguration working
                    if (string.IsNullOrEmpty(roatpV2ApiConfiguration.Url))
                        roatpV2ApiConfiguration.Url = "https://localhost:5111/";

                    ////////

                    httpClient.BaseAddress = new Uri(roatpV2ApiConfiguration.Url);


                    httpClient.DefaultRequestHeaders.Add(acceptHeaderName, acceptHeaderValue);

                    httpClient.DefaultRequestHeaders.Add("X-Version", "1"); 

                    var configuration = serviceProvider.GetService<IConfiguration>();
                    if (!configuration["EnvironmentName"].Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase))
                    {
                        var generateTokenTask = BearerTokenGenerator.GenerateTokenAsync(roatpV2ApiConfiguration.Identifier);
                        httpClient.DefaultRequestHeaders.Authorization = generateTokenTask.GetAwaiter().GetResult();
                    }
                })
                .SetHandlerLifetime(handlerLifeTime);
        }
    }
}
