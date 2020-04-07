namespace RRWebConsole
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;

    internal class Program
    {
        public static void Main(
            string[] args)
        {
            TcpListener server = null;
            try
            {
                var portSettings = InitializePortSettings();
                server = new TcpListener(
                    localaddr: IPAddress.Loopback,
                    port: portSettings.port);

                server.Start();

                while (true)
                {
                    Console.WriteLine(value: "Waiting for connections");

                    var client = server.AcceptTcpClient();

                    Console.WriteLine(value: "Client connected");

                    var stream = client.GetStream();

                    var response = ReadStream(stream: stream);

                    Console.WriteLine(value: response);

                    stream.Close();

                    client.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(value: e.Message);
            }
            finally
            {
                server?.Stop();
            }
        }

        private static string ReadStream(
            NetworkStream stream)
        {
            var result = string.Empty;
            var data = new byte[256];
            do
            {
                var readBytesCount = stream.Read(
                    buffer: data,
                    offset: 0,
                    size: data.Length);
                result += Encoding.UTF8.GetString(bytes: data, index: 0, count: readBytesCount);
            } while (stream.DataAvailable);

            return result;
        }

        private static void WriteStream(
            NetworkStream stream,
            string message)
        {
            var bytes = Encoding.UTF8.GetBytes(
                s: message);
            stream.Write(
                buffer: bytes,
                offset: 0,
                size: bytes.Length);
        }

        private static PortSettings InitializePortSettings()
        {
            Console.WriteLine(value: "Enter host port: ");
            var port = Console.ReadLine();

            if (port == null)
            {
                throw new Exception(message: "Enter correct values");
            }

            return new PortSettings(
                port: int.Parse(s: port));
        }

        private class PortSettings
        {
            public PortSettings(
                int port)
            {
                this.port = port;
            }

            public int port { get; }
        }
    }
}
