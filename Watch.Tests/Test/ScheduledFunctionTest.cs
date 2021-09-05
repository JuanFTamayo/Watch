using System;
using System.Collections.Generic;
using System.Text;
using Watch.Functions.Functions;
using Watch.Tests.Helpers;
using Xunit;

namespace Watch.Tests.Test
{
    public class ScheduledFunctionTest
    {
        [Fact]
        public void ScheduledFuction_Should_Log_Message()
        {
            // Arrange
            MockCloudTableTimes mockTimes = new MockCloudTableTimes(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            MockCloudTableConsolidated mockConsolidated = new MockCloudTableConsolidated(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            ListLogger logger = (ListLogger)TestFactory.CreateLogger(LoggerTypes.List);

            // Add
            ScheduledFunction.Run(null, mockTimes, mockConsolidated, logger);
            string message = logger.Logs[0];

            // Assert
            Assert.Contains("Get all consolidated received.", message);
        }
    }
}
