using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfozahystTest
{
    public class DataServiceTests
    {
        [Fact]
        public async Task FetchData_ShouldReturnData_WhenApiReturnsData()
        {
       
            var mockNetworkService = new Mock<INetworkService>();
            mockNetworkService.Setup(service => service.GetDataFromApiAsync(It.IsAny<string>()))
                              .ReturnsAsync("Expected Data");

            var dataService = new DataService(mockNetworkService.Object);
            var result = await dataService.FetchData("http://example.com");
            Assert.Equal("Expected Data", result);
        }

        [Fact]
        public async Task FetchData_ShouldReturnEmpty_WhenApiReturnsNull()
        {
            
            var mockNetworkService = new Mock<INetworkService>();
            mockNetworkService.Setup(service => service.GetDataFromApiAsync(It.IsAny<string>()))
                              .ReturnsAsync((string?)null);

            var dataService = new DataService(mockNetworkService.Object);
            var result = await dataService.FetchData("http://example.com");

            Assert.Null(result);
        }
    }
}
