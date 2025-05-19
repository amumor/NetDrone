using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace NetDroneClient.scripts.client
{
    public class UdpClientExample
    {
        private UdpClient _udpClient;

        public UdpClientExample()
        {
            // Initialize the UDP client
            _udpClient = new UdpClient();
        }

        public void SendMessage(string message, string serverIp, int serverPort)
        {
            try
            {
                // Convert the message to bytes
                var data = Encoding.UTF8.GetBytes(message);

                // Send the message to the server
                _udpClient.Send(data, data.Length, serverIp, serverPort);
                Console.WriteLine($"Message sent to {serverIp}:{serverPort}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message: {ex.Message}");
            }
        }

        public void StartListening(int localPort)
        {
            try
            {
                // Bind the UDP client to a local port
                _udpClient = new UdpClient(localPort);
                Console.WriteLine($"Listening on port {localPort}...");

                // Start receiving messages
                _udpClient.BeginReceive(ReceiveCallback, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error starting listener: {ex.Message}");
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Get the remote endpoint and received data
                IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] receivedData = _udpClient.EndReceive(ar, ref remoteEndPoint);

                // Convert the data to a string
                string receivedMessage = Encoding.UTF8.GetString(receivedData);
                Console.WriteLine($"Received message from {remoteEndPoint}: {receivedMessage}");

                // Continue listening for more messages
                _udpClient.BeginReceive(ReceiveCallback, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error receiving message: {ex.Message}");
            }
        }

        public void Close()
        {
            _udpClient.Close();
            Console.WriteLine("UDP client closed.");
        }
    }
}