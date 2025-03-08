// CLIENT
using System;
using System.Net.Sockets;
using System.Text;

class Client
{
    static void Main()
    {
        TcpClient tcpClient = new TcpClient();
        tcpClient.Connect("127.0.0.1", 5000);
        NetworkStream nStream = tcpClient.GetStream();

        Console.WriteLine("Enter a message to be translated:");
        string message = Console.ReadLine() ?? string.Empty;

        if (!string.IsNullOrEmpty(message))
        {
            byte[] request = Serialize(message);
            nStream.Write(request, 0, request.Length);

            string response = ReadFromStream(nStream);
            Console.WriteLine("Translated message: " + response);
        }

        tcpClient.Close();
    }

    static byte[] Serialize(string request)
    {
        byte[] requestBytes = Encoding.ASCII.GetBytes(request);
        byte requestLength = (byte)requestBytes.Length;
        byte[] rawData = new byte[requestLength + 1];
        rawData[0] = requestLength;
        requestBytes.CopyTo(rawData, 1);
        return rawData;
    }

    static string ReadFromStream(NetworkStream stream)
    {
        int responseLength = stream.ReadByte();
        byte[] responseBytes = new byte[responseLength];
        stream.Read(responseBytes, 0, responseLength);
        return Encoding.ASCII.GetString(responseBytes);
    }
}