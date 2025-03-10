using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Linq;

class Program
{
    private static readonly HttpClient client = new HttpClient();

    static async Task Main()
    {
        Console.WriteLine("Client Program Started...");

        await CallHelloEndpoint();
        await CallSortEndpoint(new int[] { 5, 2, 9, 1, 5, 6 });
    }

    private static async Task CallHelloEndpoint()
    {
        try
        {
            HttpResponseMessage response = await client.GetAsync("http://localhost:5000/api/talkback/hello");
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Hello Response: " + responseBody);
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine("HTTP Request Error calling Hello endpoint: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("General Error calling Hello endpoint: " + ex.Message);
        }
    }

    private static async Task CallSortEndpoint(int[] numbers)
    {
        try
        {
            string queryString = string.Join("&", numbers.Select(n => $"numbers={n}"));
            HttpResponseMessage response = await client.GetAsync($"http://localhost:5000/api/talkback/sort?{queryString}");
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Sort Response: " + responseBody);
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine("HTTP Request Error calling Sort endpoint: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("General Error calling Sort endpoint: " + ex.Message);
        }
    }
}