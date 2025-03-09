public interface IPipe
{
    void RegisterFilter(IFilter filter);
    IMessage ProcessMessage(IMessage message);
}