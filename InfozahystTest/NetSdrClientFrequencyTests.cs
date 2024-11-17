using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Infozahyst;

namespace InfozahystTest
{
    public class NetSdrClientFrequencyTests
    {
        private Mock<TcpClient> _mockTcpClient;
        private Mock<NetworkStream> _mockNetworkStream;
        private NetSdrClient _client;
        public NetSdrClientFrequencyTests()
        {
            _mockTcpClient = new Mock<TcpClient>();
            _mockNetworkStream = new Mock<NetworkStream>();
            _client = new NetSdrClient("localhost", 50000);
            _mockTcpClient.Setup(x => x.GetStream()).Returns(_mockNetworkStream.Object);
        }

        [Fact]
        public void SetReceiverFrequency_ShouldSendCorrectCommand_WhenFrequencyIsValid()
        {
            byte channelId = 0x00;
            double frequencyMHz = 14.010;
            ulong frequency = (ulong)(frequencyMHz * 1000000);
            byte[] expectedCommand = new byte[] { 0x0A, 0x00, 0x20, 0x00, channelId, 0xD7, 0x4C, 0x60, 0x00, 0x00 };
            _mockTcpClient.Setup(x => x.Connected).Returns(true);
            _client.SetReceiverFrequency();
            _mockNetworkStream.Verify(x => x.Write(expectedCommand, 0, expectedCommand.Length), Times.Once);
            _mockNetworkStream.Verify(x => x.Flush(), Times.Once);
        }
        [Fact]
        public void SetReceiverFrequency_ShouldThrowError_WhenFrequencyIsTooHigh()
        {
            _mockTcpClient.Setup(x => x.Connected).Returns(true);
            var exception = Record.Exception(() => _client.SetReceiverFrequency());
            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
        }
    }
}
