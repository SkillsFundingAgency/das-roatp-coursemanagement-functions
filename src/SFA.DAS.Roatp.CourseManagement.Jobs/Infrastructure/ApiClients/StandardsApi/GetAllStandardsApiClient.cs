using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Jobs.Infrastructure.ApiClients.StandardsApi.Models;

namespace SFA.DAS.Roatp.CourseManagement.Jobs.Infrastructure.ApiClients.StandardsApi
{
    public class GetAllStandardsApiClient : ApiClientBase<GetAllStandardsApiClient>, IGetAllStandardsApiClient
    {
        public GetAllStandardsApiClient(HttpClient client, ILogger<GetAllStandardsApiClient> logger)
            : base(client, logger)
        {
        }
    
        public async Task<StandardList> GetAllStandards()
        {
            return await Get<StandardList>("api/courses/Standards?Filter=None");
        }
    }
}