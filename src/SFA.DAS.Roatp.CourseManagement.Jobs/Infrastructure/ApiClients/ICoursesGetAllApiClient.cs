using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Jobs.Infrastructure.ApiClients
{
    public interface ICoursesGetAllApiClient
    {
        Task<CourseList> GetAllCourses();
    }
}
