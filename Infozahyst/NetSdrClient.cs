using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Infozahyst
{
    public class NetSdrClient
    {
        private TcpClient _tcpClient;
        private readonly string _host;
        private readonly int _port;
        private UdpClient udpClient;
        private CancellationTokenSource cancellationTokenSource;
        private bool isReceivingUdpData = false;
        private readonly ControlItemManager controlItemManager;
        private readonly UdpMessageProcessor udpMessageProcessor;

        public NetSdrClient(string host = "localhost", int port = 50000)
        {
            _host = host;
            _port = port;
            controlItemManager = new ControlItemManager();
            udpMessageProcessor = new UdpMessageProcessor(controlItemManager);
        }

        public void Connect()
        {
            if (_tcpClient != null && _tcpClient.Connected)
            {
                Console.WriteLine("Already connected.");
                return;
            }

            try
            {
                _tcpClient = new TcpClient(_host, _port);
                Console.WriteLine($"Connecting to the receiver on {_host}: {_port} successful.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection error: {ex.Message}");
            }
        }

        public void Disconnect()
        {
            if (_tcpClient == null || !_tcpClient.Connected)
            {
                Console.WriteLine("No active connection.");
                return;
            }
            try
            {
                _tcpClient.Close();
                Console.WriteLine("Disconnecting from the receiver.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Shutdown error: {ex.Message}");
            }
        }

        public void ControlReceiverState(bool start)
        {
            if (_tcpClient == null || !_tcpClient.Connected)
            {
                Console.WriteLine("No active connection.");
                return;
            }
            try
            {
                byte[] command;

                if (start)
                {
                    command = new byte[] { 0x08, 0x00, 0x18, 0x00, 0x80, 0x02, 0x80, 0x00 };
                    Console.WriteLine("Starting data capture...");
                }
                else
                {
                    command = new byte[] { 0x08, 0x00, 0x18, 0x00, 0x00, 0x01, 0x00, 0x00 };
                    Console.WriteLine("Stopping Dana's capture...");
                }
                var networkStream = _tcpClient.GetStream();
                networkStream.Write(command, 0, command.Length);
                networkStream.Flush();
                Console.WriteLine("Command sent to NetSDR.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error when managing the receiver state: {ex.Message}");
            }

            if (start)
                StartReceivingUdpData();
            else
                StopReceivingUdpData();
        }

        public void SetReceiverFrequency()
        {
            Console.WriteLine("Enter the channel ID (0 for Channel 1, 2 for Channel 2, or 0xFF for all channels):");
            byte channelId = Convert.ToByte(Console.ReadLine(), 16);
            Console.WriteLine("Enter the frequency in MHz (for example, 14.010 to 14.010 MHz):");
            double frequencyMHz = Convert.ToDouble(Console.ReadLine());
            ulong frequency = (ulong)(frequencyMHz * 1000000);

            if (_tcpClient == null || !_tcpClient.Connected)
            {
                Console.WriteLine("No active connection.");
                return;
            }

            try
            {
                if (frequency > 0xFFFFFFFFFF)
                {
                    Console.WriteLine("The frequency is too high.");
                    return;
                }
                byte[] frequencyBytes = BitConverter.GetBytes(frequency);
                Array.Reverse(frequencyBytes);
                int length = frequencyBytes.Length + 6;
                byte[] command = new byte[length];
                command[0] = 0x0A;
                command[1] = 0x00;
                command[2] = 0x20;
                command[3] = 0x00;
                command[4] = channelId;
                for (int i = 0; i < frequencyBytes.Length; i++)
                {
                    command[5 + i] = frequencyBytes[i];
                }
                Console.WriteLine($"Sending a command to change the frequency to {frequency / 1000000.0} MHz...");
                var networkStream = _tcpClient.GetStream();
                networkStream.Write(command, 0, command.Length);
                networkStream.Flush();
                Console.WriteLine("The receiver frequency has been changed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error when changing the frequency: {ex.Message}");
            }
        }

        public void StartReceivingUdpData()
        {
            if (isReceivingUdpData)
            {
                Console.WriteLine("Already receiving UDP data.");
                return;
            }
            isReceivingUdpData = true;
            cancellationTokenSource = new CancellationTokenSource();
            udpClient = new UdpClient(60000);
            Task.Run(() => ReceiveUdpData(cancellationTokenSource.Token));
        }

        public void StopReceivingUdpData()
        {
            if (!isReceivingUdpData)
            {
                Console.WriteLine("UDP receiving is not active.");
                return;
            }
            isReceivingUdpData = false;
            cancellationTokenSource.Cancel();
            udpClient.Close();
            Console.WriteLine("Stopped receiving UDP data.");
        }

        private void ReceiveUdpData(CancellationToken token)
        {
            try
            {
                using (FileStream fileStream = new FileStream("iq_data.bin", FileMode.Create, FileAccess.Write))
                {
                    while (isReceivingUdpData)
                    {
                        if (token.IsCancellationRequested)
                            break;
                        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 60000);
                        try
                        {
                            byte[] packet = udpClient.Receive(ref remoteEndPoint);
                            if (packet.Length >= 2)
                            {
                                if (packet[0] == 0x04 && (packet[1] == 0x84 || packet[1] == 0x82))
                                {
                                    int sampleCount = (packet[1] == 0x84) ? 512 : 256;
                                    for (int i = 2; i < 2 + sampleCount * 2; i += 2)
                                    {
                                        short iqSample = BitConverter.ToInt16(new byte[] { packet[i], packet[i + 1] }, 0);
                                        byte[] iqSampleBytes = BitConverter.GetBytes(iqSample);
                                        fileStream.Write(iqSampleBytes, 0, iqSampleBytes.Length);
                                    }
                                }
                                udpMessageProcessor.ProcessUdpMessage(packet);
                            }
                        }
                        catch (SocketException ex)
                        {
                            if (token.IsCancellationRequested)
                                break;
                            Console.WriteLine($"Socket error: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during receiving UDP data: {ex.Message}");
            }
        }
    }
}
