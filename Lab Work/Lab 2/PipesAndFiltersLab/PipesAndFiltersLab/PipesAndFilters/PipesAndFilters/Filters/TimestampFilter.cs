using System;
public class TimestampFilter : IFilter
{
    public IMessage Run(IMessage message)
    {
        message.Headers["Timestamp"] = DateTime.Now.ToString();
        return message;
    }
}