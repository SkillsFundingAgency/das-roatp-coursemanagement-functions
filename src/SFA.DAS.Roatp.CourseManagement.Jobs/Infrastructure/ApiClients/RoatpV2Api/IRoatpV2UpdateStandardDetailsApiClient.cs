using System.Net;
using System.Threading.Tasks;
using SFA.DAS.Roatp.CourseManagement.Jobs.Infrastructure.ApiClients.RoatpV2Api.Models;

namespace SFA.DAS.Roatp.CourseManagement.Jobs.Infrastructure.ApiClients.RoatpV2Api
{
    public interface IRoatpV2UpdateStandardDetailsApiClient
    {
        Task<HttpStatusCode> ReloadStandardsDetails(StandardsRequest standardsRequest);
    }
}
