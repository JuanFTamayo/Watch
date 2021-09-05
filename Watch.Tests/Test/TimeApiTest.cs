using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Watch.Common.Models;
using Watch.Functions.Entities;
using Watch.Functions.Functions;
using Watch.Tests.Helpers;
using Xunit;

namespace Watch.Tests.Test
{
    public class TimeApiTest
    {
        private readonly ILogger logger = TestFactory.CreateLogger();

        [Fact]
        public async void CreateTime_Should_Return_200()
        {
            // Arrenge
            MockCloudTableTimes mockTimes = new MockCloudTableTimes(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            Time timeRequest = TestFactory.GetTimeRequest();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(timeRequest);

            // Act
            IActionResult response = await TimeApi.CreateTime(request, mockTimes, logger);

            // Assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

        [Fact]
        public async void UpdateTime_Should_Return_200()
        {
            // Arrenge
            MockCloudTableTimes mockTimes = new MockCloudTableTimes(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            Time timeRequest = TestFactory.GetTimeRequest();
            Guid timeId = Guid.NewGuid();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(timeId, timeRequest);

            // Act
            IActionResult response = await TimeApi.UpdateTime(request, mockTimes, timeId.ToString(), logger);

            // Assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }
        [Fact]
        public async void DeleteTime_Should_Return_200()
        {

            // Arrange
            MockCloudTableTimes mockTimes = new MockCloudTableTimes(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            Time timeRequest = TestFactory.GetTimeRequest();
            TimeEntity timeEntity = TestFactory.GetTimeEntity();
            Guid timeId = Guid.NewGuid();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(timeId, timeRequest);

            // Act
            IActionResult response = await TimeApi.DeleteTime(request, timeEntity, mockTimes, timeId.ToString(), logger);

            // Assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

        [Fact]
        public async void GetAllTimes_Should_Return_200()
        {
            // Arrange
            MockCloudTableTimes mockTimes = new MockCloudTableTimes(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            DefaultHttpRequest request = TestFactory.CreateHttpRequest();

            // Act
            IActionResult response = await TimeApi.GetAllTimes(request, mockTimes, logger);

            // Assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

        [Fact]
        public void GetTimeById_Should_Return_200()
        {

            // Arrange
            MockCloudTableTimes mockTimes = new MockCloudTableTimes(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            TimeEntity timeEntity = TestFactory.GetTimeEntity();
            Guid timeId = Guid.NewGuid();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest();

            // Act
            IActionResult response = TimeApi.GetTimeById(request, timeEntity, timeId.ToString(), logger);

            // Assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }
    }
}
