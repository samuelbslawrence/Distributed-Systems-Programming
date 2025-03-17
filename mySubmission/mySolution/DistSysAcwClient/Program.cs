using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Threading.Tasks;

#region Task 10 and beyond

class Program
{
    private static readonly HttpClient client = new HttpClient();
    private static string storedUserName;
    private static string storedApiKey;

    static async Task Main()
    {
        Console.WriteLine("Hello. What would you like to do?");
        while (true)
        {
            string input = Console.ReadLine();
            if (input.Equals("Exit", StringComparison.OrdinalIgnoreCase))
            {
                break;
            }

            Console.Clear();
            await ProcessInput(input);
            Console.WriteLine("What would you like to do next?");
        }
    }

    private static async Task ProcessInput(string input)
    {
        string[] args = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (args.Length == 0) return;

        switch (args[0].ToLower())
        {
            case "talkback":
                if (args.Length > 1 && args[1].ToLower() == "hello")
                    await TalkBackHello();
                else if (args.Length > 2 && args[1].ToLower() == "sort")
                    await TalkBackSort(args.Skip(2).ToArray());
                break;
            case "user":
                if (args.Length > 1 && args[1].ToLower() == "get")
                    await UserGet(args[2]);
                else if (args.Length > 1 && args[1].ToLower() == "post")
                    await UserPost(args[2]);
                else if (args.Length > 1 && args[1].ToLower() == "set")
                    UserSet(args[2], args[3]);
                else if (args.Length > 1 && args[1].ToLower() == "delete")
                    await UserDelete();
                break;
            default:
                Console.WriteLine("Invalid command.");
                break;
        }
    }

    private static async Task TalkBackHello()
    {
        Console.WriteLine("...please wait...");
        HttpResponseMessage response = await client.GetAsync("http://150.237.94.9/:<5488600>/api/talkback/hello");
        Console.WriteLine(await response.Content.ReadAsStringAsync());
    }

    private static async Task TalkBackSort(string[] numbers)
    {
        Console.WriteLine("...please wait...");
        string query = string.Join("&", numbers.Select(n => $"integers={n}"));
        HttpResponseMessage response = await client.GetAsync($"http://150.237.94.9/:<5488600>/api/talkback/sort?{query}");
        Console.WriteLine(await response.Content.ReadAsStringAsync());
    }

    private static async Task UserGet(string username)
    {
        Console.WriteLine("...please wait...");
        HttpResponseMessage response = await client.GetAsync($"http://150.237.94.9/:<5488600>/api/user/new?username={username}");
        Console.WriteLine(await response.Content.ReadAsStringAsync());
    }

    private static async Task UserPost(string username)
    {
        Console.WriteLine("...please wait...");
        HttpResponseMessage response = await client.PostAsync($"http://150.237.94.9/:<5488600>/api/user/new", new StringContent(username));
        if (response.IsSuccessStatusCode)
        {
            storedUserName = username;
            storedApiKey = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Got API Key");
        }
        else
        {
            Console.WriteLine(await response.Content.ReadAsStringAsync());
        }
    }

    private static void UserSet(string username, string apiKey)
    {
        storedUserName = username;
        storedApiKey = apiKey;
        Console.WriteLine("Stored");
    }

    private static async Task UserDelete()
    {
        if (string.IsNullOrEmpty(storedUserName) || string.IsNullOrEmpty(storedApiKey))
        {
            Console.WriteLine("You need to do a User Post or User Set first");
            return;
        }
        Console.WriteLine("...please wait...");
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, $"http://150.237.94.9/:<5488600>/api/user/removeuser?username={storedUserName}");
        request.Headers.Add("ApiKey", storedApiKey);
        HttpResponseMessage response = await client.SendAsync(request);
        Console.WriteLine(await response.Content.ReadAsStringAsync());
    }
}

#endregion