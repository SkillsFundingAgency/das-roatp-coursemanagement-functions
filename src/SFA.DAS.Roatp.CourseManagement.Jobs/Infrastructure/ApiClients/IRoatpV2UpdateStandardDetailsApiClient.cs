using System.Threading.Tasks;
using SFA.DAS.Roatp.CourseManagement.Jobs.Infrastructure.RoatpV2;

namespace SFA.DAS.Roatp.CourseManagement.Jobs.Infrastructure.ApiClients
{
    public interface IRoatpV2UpdateStandardDetailsApiClient
    {
        Task<bool> ReloadStandardsDetails(StandardsRequest standardsRequest);
    }
}
