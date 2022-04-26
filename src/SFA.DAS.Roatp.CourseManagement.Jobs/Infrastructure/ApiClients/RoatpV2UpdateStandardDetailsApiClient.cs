using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Jobs.Infrastructure.RoatpV2;

namespace SFA.DAS.Roatp.CourseManagement.Jobs.Infrastructure.ApiClients
{
    public class RoatpV2UpdateStandardDetailsApiClient : ApiClientBase<RoatpV2UpdateStandardDetailsApiClient>, IRoatpV2UpdateStandardDetailsApiClient
    {
        public RoatpV2UpdateStandardDetailsApiClient(HttpClient client, ILogger<RoatpV2UpdateStandardDetailsApiClient> logger)
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