using PipesAndFilters;

class Program
{
    static void Main(string[] args)
    {
        ServerEnvironment.Setup();
        Client client = new Client();
        client.RequestHello("HELLO WORLD!");
    }
}