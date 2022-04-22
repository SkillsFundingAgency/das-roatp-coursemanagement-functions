using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Jobs.Infrastructure.ApiClients
{
    public interface IGetAllCoursesApiClient
    {
        Task<CourseList> GetAllCourses();
    }
}
