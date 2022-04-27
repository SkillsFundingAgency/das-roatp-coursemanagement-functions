using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Jobs.Infrastructure.ApiClients;
using SFA.DAS.Roatp.CourseManagement.Jobs.Infrastructure.ApiClients.RoatpV2Api;
using SFA.DAS.Roatp.CourseManagement.Jobs.Infrastructure.ApiClients.RoatpV2Api.Models;
using SFA.DAS.Roatp.CourseManagement.Jobs.Infrastructure.ApiClients.StandardsApi;

namespace SFA.DAS.Roatp.CourseManagement.Jobs.Functions
{
    public  class ReloadStandardsCacheFunction
    {
        private readonly IStandardsGetAllApiClient _standardsGetAllApiClient;
        private readonly IRoatpV2UpdateStandardDetailsApiClient _roatpV2UpdateStandardDetailsApiClient;


        public ReloadStandardsCacheFunction(IStandardsGetAllApiClient standardsGetAllApiClient, IRoatpV2UpdateStandardDetailsApiClient roatpV2UpdateStandardDetailsApiClient)
        {
            _standardsGetAllApiClient = standardsGetAllApiClient;
            _roatpV2UpdateStandardDetailsApiClient = roatpV2UpdateStandardDetailsApiClient;
        }


        [FunctionName(nameof(ReloadStandardsCacheFunction))]
        public  async Task Run([TimerTrigger("%ReloadStandardsCacheSchedule%"
    #if DEBUG
                , RunOnStartup=true
    #endif
            )] TimerInfo myTimer, ILogger log)
        {

            log.LogInformation($"ReloadStandardsCacheFunction function started");

            var standardList = await _standardsGetAllApiClient.GetAllStandards();
            var standardsRequest = new StandardsRequest { Standards = standardList.Standards };
            var result = await _roatpV2UpdateStandardDetailsApiClient.ReloadStandardsDetails(standardsRequest);
            if (result == HttpStatusCode.OK)
                log.LogInformation($"ReloadStandardsCacheFunction function completed");
            else
            {
                log.LogError($"ReloadStandardsCacheFunction function failed", result);
            }
        }
    }
}
