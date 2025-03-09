using System;
using System.Collections.Generic;
using System.Text;

namespace PipesAndFilters
{
    class TranslateFilter : IFilter
    {
        public IMessage Run(IMessage message)
        {
            if(message.Headers.ContainsKey("RequestFormat"))
            {
                switch (message.Headers["RequestFormat"])
                {
                    case "Bytes":
                        string[] byteStrings = message.Body.Split('-');
                        byte[] bytes = new byte[byteStrings.Length];
                        for (int i = 0; i < byteStrings.Length; i++)
                        {
                            bytes[i] = byte.Parse(byteStrings[i]);
                        }
                        message.Body = Encoding.ASCII.GetString(bytes);
                        break;
                    default:
                        break;
                }
            }
            else if (message.Headers.ContainsKey("ResponseFormat"))
            {
                switch (message.Headers["ResponseFormat"])
                {
                    case "Bytes":
                        byte[] bytes = Encoding.ASCII.GetBytes(message.Body);
                        string messageBody = "";
                        for (int i = 0; i < bytes.Length; i++)
                        {
                            messageBody += bytes[i].ToString();
                            if (i + 1 < bytes.Length)
                            {
                                messageBody += "-";
                            }
                        }
                        message.Body = messageBody;
                        break;
                    default:
                        break;
                }
            }
            return message;
        }
    }
}
