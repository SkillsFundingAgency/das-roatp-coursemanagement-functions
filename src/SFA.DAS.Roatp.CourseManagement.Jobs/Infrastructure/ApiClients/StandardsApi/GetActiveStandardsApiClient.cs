using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Jobs.Infrastructure.ApiClients.StandardsApi.Models;

namespace SFA.DAS.Roatp.CourseManagement.Jobs.Infrastructure.ApiClients.StandardsApi
{
    public class GetActiveStandardsApiClient : ApiClientBase<GetActiveStandardsApiClient>, IGetActiveStandardsApiClient
    {
        public GetActiveStandardsApiClient(HttpClient client, ILogger<GetActiveStandardsApiClient> logger)
            : base(client, logger)
        {
        }
    
        public async Task<StandardList> GetActiveStandards()
        {
            return await Get<StandardList>("api/courses/Standards?Filter=Active");
        }
    }
}