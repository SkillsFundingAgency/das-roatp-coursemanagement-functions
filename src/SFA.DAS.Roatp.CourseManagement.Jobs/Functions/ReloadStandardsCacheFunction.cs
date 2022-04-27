using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Jobs.Infrastructure.ApiClients.RoatpV2Api;
using SFA.DAS.Roatp.CourseManagement.Jobs.Infrastructure.ApiClients.RoatpV2Api.Models;
using SFA.DAS.Roatp.CourseManagement.Jobs.Infrastructure.ApiClients.StandardsApi;

namespace SFA.DAS.Roatp.CourseManagement.Jobs.Functions
{
    public  class ReloadStandardsCacheFunction
    {
        private readonly IGetAllStandardsApiClient _getAllStandardsApiClient;
        private readonly IReloadStandardsApiClient _reloadStandardsApiClient;


        public ReloadStandardsCacheFunction(IGetAllStandardsApiClient getAllStandardsApiClient, IReloadStandardsApiClient reloadStandardsApiClient)
        {
            _getAllStandardsApiClient = getAllStandardsApiClient;
            _reloadStandardsApiClient = reloadStandardsApiClient;
        }


        [FunctionName(nameof(ReloadStandardsCacheFunction))]
        public  async Task Run([TimerTrigger("%ReloadStandardsCacheSchedule%")] TimerInfo myTimer, ILogger log)
        {

            log.LogInformation($"ReloadStandardsCacheFunction function started");

            var standardList = await _getAllStandardsApiClient.GetAllStandards();
            var standardsRequest = new StandardsRequest { Standards = standardList.Standards };
            var result = await _reloadStandardsApiClient.ReloadStandardsDetails(standardsRequest);
            if (result == HttpStatusCode.OK)
                log.LogInformation($"ReloadStandardsCacheFunction function completed");
            else
            {
                log.LogError($"ReloadStandardsCacheFunction function failed", result);
            }
        }
    }
}
