using System;
using System.Net.Sockets;
using System.Text;

TcpClient tcpClient = new TcpClient();
tcpClient.Connect("127.0.0.1", 5000);
NetworkStream nStream = tcpClient.GetStream();

Console.WriteLine("Enter a message to be translated...");
string? message = Console.ReadLine();

if (message != null)
{
    byte[] request = Serialize(message);
    nStream.Write(request, 0, request.Length);
    // TODO: Read response from stream and display to user
}

Console.ReadKey(); // Wait for keypress before exit

byte[] Serialize(string request)
{
    byte[] responseBytes = Encoding.ASCII.GetBytes(request);
    byte responseLength = (byte)responseBytes.Length;
    
    byte[] rawData = new byte[responseLength + 1];
    rawData[0] = responseLength;
    responseBytes.CopyTo(rawData, 1);

    return rawData;
}