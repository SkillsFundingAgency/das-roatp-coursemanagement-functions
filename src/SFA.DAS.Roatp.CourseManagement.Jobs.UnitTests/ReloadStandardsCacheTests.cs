using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Timers;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Jobs.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Jobs.Infrastructure.ApiClients;
using SFA.DAS.Roatp.CourseManagement.Jobs.Infrastructure.RoatpV2;

namespace SFA.DAS.Roatp.CourseManagement.Jobs.UnitTests
{
    public class ReloadStandardsCacheTests
    {
        private Mock<IStandardsGetAllApiClient> _standardsGetAllApiClient;
        private Mock<IRoatpV2UpdateStandardDetailsApiClient> _roatpV2UpdateStandardDetailsApiClient;
        private Mock<ILogger<ReloadStandardsCache>> _logger;
        private ReloadStandardsCache _function;


        [SetUp]
        public void Setup()
        {
            _logger = new Mock<ILogger<ReloadStandardsCache>>();
            _standardsGetAllApiClient = new Mock<IStandardsGetAllApiClient>();
            _roatpV2UpdateStandardDetailsApiClient = new Mock<IRoatpV2UpdateStandardDetailsApiClient>();
            _function = new ReloadStandardsCache(_standardsGetAllApiClient.Object, _roatpV2UpdateStandardDetailsApiClient.Object);
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
            _logger.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.AtLeastOnce);
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
            _logger.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.AtLeastOnce);
        }
    }
}
