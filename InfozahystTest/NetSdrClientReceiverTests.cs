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
    public class NetSdrClientReceiverTests
    {
        private Mock<TcpClient> _mockTcpClient;
        private Mock<NetworkStream> _mockNetworkStream;
        private NetSdrClient _client;

        public NetSdrClientReceiverTests()
        {
            _mockTcpClient = new Mock<TcpClient>();
            _mockNetworkStream = new Mock<NetworkStream>();
            _client = new NetSdrClient("localhost", 50000);
            _mockTcpClient.Setup(x => x.GetStream()).Returns(_mockNetworkStream.Object);
        }
        [Fact]
        public void ControlReceiverState_ShouldSendStartCommand_WhenStartIsTrue()
        {
            byte[] expectedCommand = new byte[] { 0x08, 0x00, 0x18, 0x00, 0x80, 0x02, 0x80, 0x00 };
            _mockTcpClient.Setup(x => x.Connected).Returns(true);
            _client.ControlReceiverState(true);
            _mockNetworkStream.Verify(x => x.Write(expectedCommand, 0, expectedCommand.Length), Times.Once);
            _mockNetworkStream.Verify(x => x.Flush(), Times.Once);
        }

        [Fact]
        public void ControlReceiverState_ShouldSendStopCommand_WhenStartIsFalse()
        {
            byte[] expectedCommand = new byte[] { 0x08, 0x00, 0x18, 0x00, 0x00, 0x01, 0x00, 0x00 };
            _mockTcpClient.Setup(x => x.Connected).Returns(true);
            _client.ControlReceiverState(false);
            _mockNetworkStream.Verify(x => x.Write(expectedCommand, 0, expectedCommand.Length), Times.Once);
            _mockNetworkStream.Verify(x => x.Flush(), Times.Once);
        }

        [Fact]
        public void ControlReceiverState_ShouldHandleError_WhenStreamWriteFails()
        {
            _mockTcpClient.Setup(x => x.Connected).Returns(true);
            _mockNetworkStream.Setup(x => x.Write(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>())).Throws(new IOException("Write error"));
            var exception = Record.Exception(() => _client.ControlReceiverState(true));
            Assert.NotNull(exception);
            Assert.IsType<IOException>(exception);
        }
    }
}
