using System.Collections.Generic;
public class Pipe : IPipe
{
    private List<IFilter> Filters;

    public Pipe()
    {
        Filters = new List<IFilter>();
    }

    public void RegisterFilter(IFilter filter)
    {
        Filters.Add(filter);
    }

    public IMessage ProcessMessage(IMessage message)
    {
        foreach (var filter in Filters)
        {
            message = filter.Run(message);
        }
        return message;
    }
}