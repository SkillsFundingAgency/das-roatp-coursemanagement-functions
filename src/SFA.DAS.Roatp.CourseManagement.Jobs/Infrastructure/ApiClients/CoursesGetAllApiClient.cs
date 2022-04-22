using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.Roatp.CourseManagement.Jobs.Infrastructure.ApiClients
{
    public class CoursesGetAllApiClient : ApiClientBase<CoursesGetAllApiClient>, ICoursesGetAllApiClient
    {
        public CoursesGetAllApiClient(HttpClient client, ILogger<CoursesGetAllApiClient> logger)
            : base(client, logger)
        {
        }
    
        public async Task<CourseList> GetAllCourses()
        {
            return await Get<CourseList>("api/courses/Standards");
        }
    
    }
}