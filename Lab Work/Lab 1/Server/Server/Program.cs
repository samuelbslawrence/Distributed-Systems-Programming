// SERVER
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

class Server
{
    static void Main()
    {
        TcpListener tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 5000);
        tcpListener.Start();
        Console.WriteLine("Server started. Waiting for connection...");

        TcpClient tcpClient = tcpListener.AcceptTcpClient();
        NetworkStream nStream = tcpClient.GetStream();

        string message = ReadFromStream(nStream);
        Console.WriteLine("Received: \"" + message + "\"");

        string translatedMessage = Translate(message);
        Console.WriteLine("Translated: \"" + translatedMessage + "\"");

        byte[] response = Serialize(translatedMessage);
        nStream.Write(response, 0, response.Length);

        tcpClient.Close();
        tcpListener.Stop();
    }

    static string Translate(string message)
    {
        string[] words = message.Split(' ');
        StringBuilder translatedMessage = new StringBuilder();

        foreach (string word in words)
        {
            string translatedWord = ConvertToPigLatin(word);
            translatedMessage.Append(translatedWord + " ");
        }

        return translatedMessage.ToString().Trim();
    }

    static string ConvertToPigLatin(string word)
    {
        if (string.IsNullOrEmpty(word)) return word;

        string vowels = "AEIOUaeiou";
        int index = 0;

        while (index < word.Length && !vowels.Contains(word[index]))
            index++;

        if (index == 0)
            return word + "way";
        else
            return word.Substring(index) + word.Substring(0, index) + "ay";
    }

    static string ReadFromStream(NetworkStream stream)
    {
        int messageLength = stream.ReadByte();
        byte[] messageBytes = new byte[messageLength];
        stream.Read(messageBytes, 0, messageLength);
        return Encoding.ASCII.GetString(messageBytes);
    }

    static byte[] Serialize(string response)
    {
        byte[] responseBytes = Encoding.ASCII.GetBytes(response);
        byte responseLength = (byte)responseBytes.Length;
        byte[] rawData = new byte[responseLength + 1];
        rawData[0] = responseLength;
        responseBytes.CopyTo(rawData, 1);
        return rawData;
    }
}