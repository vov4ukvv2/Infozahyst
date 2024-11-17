using Infozahyst;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace InfozahystTest
{
    public class NetSdrClientTests
    {
        private Mock<TcpClient> _mockTcpClient;
        private NetSdrClient _client;

        public NetSdrClientTests()
        {
            _mockTcpClient = new Mock<TcpClient>();
            _client = new NetSdrClient("localhost", 50000);
        }

        [Fact]
        public void Connect_ShouldConnectSuccessfully_WhenTcpClientIsNotConnected()
        {
            _mockTcpClient.Setup(x => x.Connected).Returns(false);
            _client.Connect();
            _mockTcpClient.Verify(x => x.Connect(It.IsAny<string>(), It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public void Connect_ShouldPrintError_WhenConnectionFails()
        {
            _mockTcpClient.Setup(x => x.Connect(It.IsAny<string>(), It.IsAny<int>())).Throws(new Exception("Connection error"));
            var exception = Record.Exception(() => _client.Connect());
            Assert.NotNull(exception);
            Assert.IsType<Exception>(exception);
        }

        [Fact]
        public void Disconnect_ShouldDisconnectSuccessfully_WhenTcpClientIsConnected()
        {
            _mockTcpClient.Setup(x => x.Connected).Returns(true);
            _client.Disconnect();
            _mockTcpClient.Verify(x => x.Close(), Times.Once);
        }
    }
}
