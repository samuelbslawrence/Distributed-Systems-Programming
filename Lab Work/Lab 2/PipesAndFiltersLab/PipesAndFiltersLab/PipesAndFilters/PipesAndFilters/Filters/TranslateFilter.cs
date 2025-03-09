using System;
using System.Text;
public class TranslateFilter : IFilter
{
    public IMessage Run(IMessage message)
    {
        if (message.Headers.ContainsKey("RequestFormat"))
        {
            string format = message.Headers["RequestFormat"];
            if (format == "Bytes")
            {
                // Convert from byte format to ASCII string
                string[] byteStrings = message.Body.Split('-');
                byte[] bytes = new byte[byteStrings.Length];
                for (int i = 0; i < byteStrings.Length; i++)
                {
                    bytes[i] = byte.Parse(byteStrings[i]);
                }
                message.Body = Encoding.ASCII.GetString(bytes);
            }
        }

        if (message.Headers.ContainsKey("ResponseFormat"))
        {
            string format = message.Headers["ResponseFormat"];
            if (format == "Bytes")
            {
                // Convert from ASCII string to byte format
                byte[] bytes = Encoding.ASCII.GetBytes(message.Body);
                string responseBody = "";
                for (int i = 0; i < bytes.Length; i++)
                {
                    responseBody += bytes[i].ToString();
                    if (i + 1 < bytes.Length)
                    {
                        responseBody += "-";
                    }
                }
                message.Body = responseBody;
            }
        }

        return message;
    }
}