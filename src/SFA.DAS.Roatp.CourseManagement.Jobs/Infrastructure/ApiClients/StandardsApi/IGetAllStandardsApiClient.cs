﻿using System.Threading.Tasks;
using SFA.DAS.Roatp.CourseManagement.Jobs.Infrastructure.ApiClients.StandardsApi.Models;

namespace SFA.DAS.Roatp.CourseManagement.Jobs.Infrastructure.ApiClients.StandardsApi
{
    public interface IGetAllStandardsApiClient
    {
        Task<StandardList> GetAllStandards();
    }
}
