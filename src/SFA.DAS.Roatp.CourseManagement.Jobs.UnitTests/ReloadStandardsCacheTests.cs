using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Timers;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Jobs.Functions;
using SFA.DAS.Roatp.CourseManagement.Jobs.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Jobs.Infrastructure.ApiClients;
using SFA.DAS.Roatp.CourseManagement.Jobs.Infrastructure.ApiClients.RoatpV2Api;
using SFA.DAS.Roatp.CourseManagement.Jobs.Infrastructure.ApiClients.RoatpV2Api.Models;
using SFA.DAS.Roatp.CourseManagement.Jobs.Infrastructure.ApiClients.StandardsApi;
using SFA.DAS.Roatp.CourseManagement.Jobs.Infrastructure.ApiClients.StandardsApi.Models;

namespace SFA.DAS.Roatp.CourseManagement.Jobs.UnitTests
{
    public class ReloadStandardsCacheTests
    {
        private Mock<IGetAllStandardsApiClient> _standardsGetAllApiClient;
        private Mock<IReloadStandardsApiClient> _roatpV2UpdateStandardDetailsApiClient;
        private Mock<ILogger<ReloadStandardsCacheFunction>> _logger;
        private ReloadStandardsCacheFunction _function;


        [SetUp]
        public void Setup()
        {
            _logger = new Mock<ILogger<ReloadStandardsCacheFunction>>();
            _standardsGetAllApiClient = new Mock<IGetAllStandardsApiClient>();
            _roatpV2UpdateStandardDetailsApiClient = new Mock<IReloadStandardsApiClient>();
            _function = new ReloadStandardsCacheFunction(_standardsGetAllApiClient.Object, _roatpV2UpdateStandardDetailsApiClient.Object);
        }

        [Test]
        public async Task Run_Successful_Logs_Information_Message()
        {
            var standards = new List<Standard> { new Standard {StandardUid = "1", IfateReferenceNumber = "2", LarsCode = 3, Level = "4",Title = "course title 5", Version = "1.1"}};
            var standardList = new StandardList
            {
                Standards = standards
            };

            _standardsGetAllApiClient.Setup(x => x.GetAllStandards()).ReturnsAsync(standardList);
            _roatpV2UpdateStandardDetailsApiClient.Setup(x => x.ReloadStandardsDetails(It.IsAny<StandardsRequest>()))
                .ReturnsAsync(HttpStatusCode.OK);

            var timerInfo = new TimerInfo(new ConstantSchedule(TimeSpan.Zero),new ScheduleStatus(),true);
            await _function.Run(timerInfo,_logger.Object);
            _logger.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Exactly(2));
        }

        [Test]
        public async Task Run_Unsuccessful_Logs_Error_Message()
        {
            var standards = new List<Standard> { new Standard { StandardUid = "1", IfateReferenceNumber = "2", LarsCode = 3, Level = "4", Title = "course title 5", Version = "1.1" } };
            var standardList = new StandardList
            {
                Standards = standards
            };

            _standardsGetAllApiClient.Setup(x => x.GetAllStandards()).ReturnsAsync(standardList);
            _roatpV2UpdateStandardDetailsApiClient.Setup(x => x.ReloadStandardsDetails(It.IsAny<StandardsRequest>()))
                .ReturnsAsync(HttpStatusCode.BadRequest);

            var timerInfo = new TimerInfo(new ConstantSchedule(TimeSpan.Zero), new ScheduleStatus(), true);
            await _function.Run(timerInfo, _logger.Object);
            _logger.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
            _logger.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
        }
    }
}
