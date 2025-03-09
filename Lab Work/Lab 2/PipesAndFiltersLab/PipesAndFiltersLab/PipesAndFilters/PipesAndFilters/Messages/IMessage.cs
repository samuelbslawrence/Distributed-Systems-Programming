using System.Collections.Generic;
public interface IMessage
{
    Dictionary<string, string> Headers { get; set; }
    string Body { get; set; }
}