using System.Threading.Tasks;
using SFA.DAS.Roatp.CourseManagement.Jobs.Infrastructure.RoatpV2;

namespace SFA.DAS.Roatp.CourseManagement.Jobs.Infrastructure.ApiClients
{
    public interface IRoatpV2UpdateCourseDetailsApiClient
    {
        Task<bool> UpdateCoursesDetails(CoursesRequest coursesRequest);
    }
}
