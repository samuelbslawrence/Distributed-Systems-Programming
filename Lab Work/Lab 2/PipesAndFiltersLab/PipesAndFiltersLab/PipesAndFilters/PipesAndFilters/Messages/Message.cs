using System.Collections.Generic;

public class Message : IMessage
{
    public Dictionary<string, string> Headers { get; set; }
    public string Body { get; set; }

    public Message()
    {
        Headers = new Dictionary<string, string>();
    }
}