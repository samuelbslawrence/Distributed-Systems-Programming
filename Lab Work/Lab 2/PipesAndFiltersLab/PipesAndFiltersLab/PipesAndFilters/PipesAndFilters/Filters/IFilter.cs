using PipesAndFilters;

public interface IFilter
{
    IMessage Run(IMessage message);
}