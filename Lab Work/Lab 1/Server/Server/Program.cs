using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

TcpListener tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 5000);
tcpListener.Start();
TcpClient tcpClient = tcpListener.AcceptTcpClient(); NetworkStream nStream =
tcpClient.GetStream();

string message = ReadFromStream(nStream);
Console.WriteLine("Received: \"" + message + "\"");

string translatedMessage = Translate(message);

// TODO: Serialize the translated message and write it to the stream

Console.ReadKey(); // Wait for keypress before exit

static string Translate(string message)
{
    string translatedmessage = "TEST RESPONSE";
    string[] words = message.Split(' ');
    foreach (string word in words)
    {
        // TODO: Perform translation
    }
    return translatedmessage;
}

static string ReadFromStream(NetworkStream stream)
{
    int messageLength = stream.ReadByte();
    byte[] messageBytes = new byte[messageLength];
    stream.Read(messageBytes, 0, messageLength);
    return Encoding.ASCII.GetString(messageBytes);
}