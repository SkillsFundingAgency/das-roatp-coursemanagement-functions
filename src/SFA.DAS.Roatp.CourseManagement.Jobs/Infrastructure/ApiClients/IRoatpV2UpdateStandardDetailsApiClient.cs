using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.CourseManagement.Jobs.Infrastructure.RoatpV2;

namespace SFA.DAS.Roatp.CourseManagement.Jobs.Infrastructure.ApiClients
{
    public interface IRoatpV2UpdateStandardDetailsApiClient
    {
        Task<HttpStatusCode> ReloadStandardsDetails(StandardsRequest standardsRequest);
    }
}
