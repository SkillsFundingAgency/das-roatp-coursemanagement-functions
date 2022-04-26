using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Jobs.Functions;
using SFA.DAS.Roatp.CourseManagement.Jobs.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Jobs.Infrastructure.ApiClients;
using SFA.DAS.Roatp.CourseManagement.Jobs.Infrastructure.RoatpV2;

namespace SFA.DAS.Roatp.CourseManagement.Jobs
{
    public  class ReloadStandardsCache
    {
        private readonly IStandardsGetAllApiClient _standardsGetAllApiClient;
        private readonly IRoatpV2UpdateStandardDetailsApiClient _roatpV2UpdateStandardDetailsApiClient;


        public ReloadStandardsCache(IStandardsGetAllApiClient standardsGetAllApiClient, IRoatpV2UpdateStandardDetailsApiClient roatpV2UpdateStandardDetailsApiClient)
        {
            _standardsGetAllApiClient = standardsGetAllApiClient;
            _roatpV2UpdateStandardDetailsApiClient = roatpV2UpdateStandardDetailsApiClient;
        }


        [FunctionName(nameof(ReloadStandardsCache))]
        //public  async Task Run([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer, ILogger log)
        public async Task Run([TimerTrigger("0 */5 * * * *"
#if DEBUG
            , RunOnStartup=true
#endif
            )] TimerInfo myTimer, ILogger log)
        {

            log.LogInformation($"ReloadStandardsCache function started");

            var standardList = await _standardsGetAllApiClient.GetAllStandards();
            var standardsRequest = new StandardsRequest { Standards = standardList.Standards };
            var result = await _roatpV2UpdateStandardDetailsApiClient.ReloadStandardsDetails(standardsRequest);
            if (result == HttpStatusCode.OK)
                log.LogInformation($"ReloadStandardsCache function completed");
            else
            {
                log.LogError($"ReloadStandardsCache function failed", result);
            }
        }
    }
}
