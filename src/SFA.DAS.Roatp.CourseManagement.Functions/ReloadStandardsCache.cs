using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.Roatp.CourseManagement.Functions
{
    public static class ReloadStandardsCache
    {
        [FunctionName(nameof(ReloadStandardsCache))]
        public static void Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}
