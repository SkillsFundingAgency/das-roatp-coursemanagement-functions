using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Jobs.Infrastructure.RoatpV2;

namespace SFA.DAS.Roatp.CourseManagement.Jobs.Infrastructure.ApiClients
{
    public class RoatpV2UpdateCourseDetailsApiClient : ApiClientBase<RoatpV2UpdateCourseDetailsApiClient>, IRoatpV2UpdateCourseDetailsApiClient
    {
        public RoatpV2UpdateCourseDetailsApiClient(HttpClient client, ILogger<RoatpV2UpdateCourseDetailsApiClient> logger)
            : base(client, logger)
        {
        }

        public async  Task<bool> UpdateCoursesDetails(CoursesRequest coursesRequest)
        {
            var url = "ReloadCoursesData";
            return await Post<CoursesRequest, bool>(url, coursesRequest);
        }
    }
}