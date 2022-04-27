using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Jobs.Infrastructure.ApiClients.RoatpV2Api.Models;

namespace SFA.DAS.Roatp.CourseManagement.Jobs.Infrastructure.ApiClients.RoatpV2Api
{
    public class ReloadStandardsApiClient : ApiClientBase<ReloadStandardsApiClient>, IReloadStandardsApiClient
    {
        public ReloadStandardsApiClient(HttpClient client, ILogger<ReloadStandardsApiClient> logger)
            : base(client, logger)
        {
        }

        public async  Task<HttpStatusCode> ReloadStandardsDetails(StandardsRequest standardsRequest)
        {
            var url = "ReloadStandardsData";
           return  await Post<StandardsRequest>(url, standardsRequest);
        }
    }
}