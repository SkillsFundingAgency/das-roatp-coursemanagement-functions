using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Jobs.Functions;
using SFA.DAS.Roatp.CourseManagement.Jobs.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Jobs.Infrastructure.ApiClients;
using SFA.DAS.Roatp.CourseManagement.Jobs.Infrastructure.RoatpV2;

namespace SFA.DAS.Roatp.CourseManagement.Jobs
{
    public  class ReloadStandardsCache
    {
        private readonly IGetAllCoursesApiClient _getAllCoursesApiClient;
        private readonly IRoatpV2UpdateCourseDetailsApiClient _roatpV2UpdateCourseDetailsApiClient;


        public ReloadStandardsCache(IGetAllCoursesApiClient getAllCoursesApiClient, IRoatpV2UpdateCourseDetailsApiClient roatpV2UpdateCourseDetailsApiClient)
        {
            _getAllCoursesApiClient = getAllCoursesApiClient;
            _roatpV2UpdateCourseDetailsApiClient = roatpV2UpdateCourseDetailsApiClient;
        }


        [FunctionName(nameof(ReloadStandardsCache))]
        //public  async Task Run([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer, ILogger log)
        public async Task Run([TimerTrigger("0 */5 * * * *"
#if DEBUG
            , RunOnStartup=true
#endif
            )] TimerInfo myTimer, ILogger log)
        {

            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            var courseList = await _getAllCoursesApiClient.GetAllCourses();
            var coursesRequest = new CoursesRequest { Courses = courseList.Standards };
            var result = await _roatpV2UpdateCourseDetailsApiClient.UpdateCoursesDetails(coursesRequest);

        }
    }
}
