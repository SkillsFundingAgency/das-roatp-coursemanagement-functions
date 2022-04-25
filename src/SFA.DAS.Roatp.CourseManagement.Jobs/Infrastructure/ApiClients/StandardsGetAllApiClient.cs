using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.Roatp.CourseManagement.Jobs.Infrastructure.ApiClients
{
    public class StandardsGetAllApiClient : ApiClientBase<StandardsGetAllApiClient>, IStandardsGetAllApiClient
    {
        public StandardsGetAllApiClient(HttpClient client, ILogger<StandardsGetAllApiClient> logger)
            : base(client, logger)
        {
        }
    
        public async Task<StandardList> GetAllStandards()
        {
            return await Get<StandardList>("api/courses/Standards?Filter=None");
        }
    }
}