using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Security.Cryptography;

class Program
{
    #region Task 10
    // Change to switch between the test server and local server
    private const string BaseUrl = "http://150.237.94.9/5488600/api/";
    private static readonly HttpClient client = new HttpClient();

    private static string storedUserName = "";
    private static string storedApiKey = "";
    private static string storedPublicKey = ""; // Stores the server's public RSA key

    static async Task Main()
    {
        // Initial prompt exactly as specified
        Console.WriteLine("Hello. What would you like to do?");
        while (true)
        {
            // Wait for user input without clearing the console yet
            string input = Console.ReadLine();
            if (input.Equals("Exit", StringComparison.OrdinalIgnoreCase))
            {
                break;
            }

            // Clear the console only after the user has entered a command
            Console.Clear();
            await ProcessInput(input);

            // After processing, prompt for the next command exactly
            Console.WriteLine("\nWhat would you like to do next?");
        }
    }

    private static async Task ProcessInput(string input)
    {
        // Split input into arguments using whitespace
        string[] args = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (args.Length == 0)
        {
            return;
        }

        try
        {
            string command = args[0].ToLower();
            switch (command)
            {
                case "talkback":
                    if (args.Length > 1)
                    {
                        string subCommand = args[1].ToLower();
                        if (subCommand == "hello")
                        {
                            await TalkBackHello();
                        }
                        else if (subCommand == "sort")
                        {
                            if (args.Length > 2)
                            {
                                await TalkBackSort(args.Skip(2).ToArray());
                            }
                            else
                            {
                                Console.WriteLine("Please provide a list of integers to sort.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid talkback command.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Incomplete talkback command.");
                    }
                    break;

                case "user":
                    if (args.Length > 1)
                    {
                        string subCommand = args[1].ToLower();
                        if (subCommand == "get")
                        {
                            if (args.Length > 2)
                                await UserGet(args[2]);
                            else
                                Console.WriteLine("Please provide a username for User Get.");
                        }
                        else if (subCommand == "post")
                        {
                            if (args.Length > 2)
                                await UserPost(args[2]);
                            else
                                Console.WriteLine("Please provide a username for User Post.");
                        }
                        else if (subCommand == "set")
                        {
                            if (args.Length > 3)
                                UserSet(args[2], args[3]);
                            else
                                Console.WriteLine("Please provide username and API key for User Set.");
                        }
                        else if (subCommand == "delete")
                        {
                            await UserDelete();
                        }
                        else if (subCommand == "role")
                        {
                            if (args.Length > 3)
                                await UserRole(args[2], args[3]);
                            else
                                Console.WriteLine("Please provide a username and role for User Role.");
                        }
                        else
                        {
                            Console.WriteLine("Invalid user command.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Incomplete user command.");
                    }
                    break;

                case "protected":
                    if (args.Length > 1)
                    {
                        string subCommand = args[1].ToLower();
                        if (subCommand == "hello")
                        {
                            await ProtectedHello();
                        }
                        else if (subCommand == "sha1")
                        {
                            if (args.Length > 2)
                                await ProtectedSha1(args[2]);
                            else
                                Console.WriteLine("Please provide a message for Protected SHA1.");
                        }
                        else if (subCommand == "sha256")
                        {
                            if (args.Length > 2)
                                await ProtectedSha256(args[2]);
                            else
                                Console.WriteLine("Please provide a message for Protected SHA256.");
                        }
                        else if (subCommand == "getpublickey")
                        {
                            await ProtectedGetPublicKey();
                        }
                        else if (subCommand == "sign")
                        {
                            if (args.Length > 2)
                            {
                                // Allow messages with spaces
                                string message = string.Join(" ", args.Skip(2));
                                await ProtectedSign(message);
                            }
                            else
                            {
                                Console.WriteLine("Please provide a message for Protected Sign.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid protected command.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Incomplete protected command.");
                    }
                    break;

                default:
                    Console.WriteLine("Invalid command.");
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred while processing the command: " + ex.Message);
        }
    }

    private static async Task TalkBackHello()
    {
        Console.WriteLine("...please wait...");
        try
        {
            HttpResponseMessage response = await client.GetAsync($"{BaseUrl}talkback/hello");
            await PrintResponse(response);
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine("Network error: " + ex.Message);
        }
    }

    private static async Task TalkBackSort(string[] numbers)
    {
        Console.WriteLine("...please wait...");
        try
        {
            string query = string.Join("&", numbers.Select(n => $"integers={n}"));
            HttpResponseMessage response = await client.GetAsync($"{BaseUrl}talkback/sort?{query}");
            await PrintResponse(response);
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine("Network error: " + ex.Message);
        }
    }

    private static async Task UserGet(string username)
    {
        Console.WriteLine("...please wait...");
        try
        {
            HttpResponseMessage response = await client.GetAsync($"{BaseUrl}user/new?username={username}");
            await PrintResponse(response);
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine("Network error: " + ex.Message);
        }
    }

    private static async Task UserPost(string username)
    {
        Console.WriteLine("...please wait...");
        try
        {
            // Send username as JSON in the request body
            var content = new StringContent(JsonSerializer.Serialize(username), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync($"{BaseUrl}user/new", content);
            if (response.IsSuccessStatusCode)
            {
                storedUserName = username;
                // Store the API key but do not display it
                storedApiKey = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Got API Key");
            }
            else
            {
                await PrintResponse(response);
            }
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine("Network error: " + ex.Message);
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
        try
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, $"{BaseUrl}user/removeuser?username={storedUserName}");
            request.Headers.Add("ApiKey", storedApiKey);
            HttpResponseMessage response = await client.SendAsync(request);
            // Output exactly "True" if deletion succeeded, otherwise "False"
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("True");
            }
            else
            {
                Console.WriteLine("False");
            }
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine("Network error: " + ex.Message);
        }
    }

    private static async Task UserRole(string username, string role)
    {
        if (string.IsNullOrEmpty(storedApiKey))
        {
            Console.WriteLine("You need to do a User Post or User Set first");
            return;
        }
        Console.WriteLine("...please wait...");
        try
        {
            var jsonObj = new { username = username, role = role };
            var json = JsonSerializer.Serialize(jsonObj);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"{BaseUrl}user/changerole");
            request.Headers.Add("ApiKey", storedApiKey);
            request.Content = content;

            HttpResponseMessage response = await client.SendAsync(request);
            await PrintResponse(response);
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine("Network error: " + ex.Message);
        }
    }

    private static async Task ProtectedHello()
    {
        if (string.IsNullOrEmpty(storedApiKey))
        {
            Console.WriteLine("You need to do a User Post or User Set first");
            return;
        }
        Console.WriteLine("...please wait...");
        try
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{BaseUrl}protected/hello");
            request.Headers.Add("ApiKey", storedApiKey);
            HttpResponseMessage response = await client.SendAsync(request);
            await PrintResponse(response);
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine("Network error: " + ex.Message);
        }
    }

    private static async Task ProtectedSha1(string message)
    {
        if (string.IsNullOrEmpty(storedApiKey))
        {
            Console.WriteLine("You need to do a User Post or User Set first");
            return;
        }
        Console.WriteLine("...please wait...");
        try
        {
            string encodedMessage = Uri.EscapeDataString(message);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{BaseUrl}protected/sha1?message={encodedMessage}");
            request.Headers.Add("ApiKey", storedApiKey);
            HttpResponseMessage response = await client.SendAsync(request);
            await PrintResponse(response);
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine("Network error: " + ex.Message);
        }
    }

    private static async Task ProtectedSha256(string message)
    {
        if (string.IsNullOrEmpty(storedApiKey))
        {
            Console.WriteLine("You need to do a User Post or User Set first");
            return;
        }
        Console.WriteLine("...please wait...");
        try
        {
            string encodedMessage = Uri.EscapeDataString(message);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{BaseUrl}protected/sha256?message={encodedMessage}");
            request.Headers.Add("ApiKey", storedApiKey);
            HttpResponseMessage response = await client.SendAsync(request);
            await PrintResponse(response);
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine("Network error: " + ex.Message);
        }
    }
    #endregion

    #region Task 11
    // Getting and storing the server's public RSA key
    private static async Task ProtectedGetPublicKey()
    {
        if (string.IsNullOrEmpty(storedApiKey))
        {
            Console.WriteLine("You need to do a User Post or User Set first");
            return;
        }
        Console.WriteLine("...please wait...");
        try
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{BaseUrl}protected/getpublickey");
            request.Headers.Add("ApiKey", storedApiKey);
            HttpResponseMessage response = await client.SendAsync(request);
            string responseText = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                storedPublicKey = responseText; // store the public RSA key
                Console.WriteLine("Got Public Key");
            }
            else
            {
                Console.WriteLine("Couldn't Get the Public Key");
            }
        }
        catch (HttpRequestException)
        {
            Console.WriteLine("Couldn't Get the Public Key");
        }
    }
    #endregion

    #region Task 12
    // Digitally signing a message and verifying the signature using the server's public RSA key
    private static async Task ProtectedSign(string message)
    {
        if (string.IsNullOrEmpty(storedApiKey))
        {
            Console.WriteLine("You need to do a User Post or User Set first");
            return;
        }
        if (string.IsNullOrEmpty(storedPublicKey))
        {
            Console.WriteLine("Client doesn't yet have the public key");
            return;
        }
        Console.WriteLine("...please wait...");
        try
        {
            string encodedMessage = Uri.EscapeDataString(message);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{BaseUrl}protected/sign?message={encodedMessage}");
            request.Headers.Add("ApiKey", storedApiKey);
            HttpResponseMessage response = await client.SendAsync(request);
            string signatureHex = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Message was not successfully signed");
                return;
            }

            // Convert the hex string with dashes to a byte array
            byte[] signatureBytes = HexStringToByteArray(signatureHex);

            // Use the server's public key to verify the signature
            using (var rsa = new RSACryptoServiceProvider())
            {
                try
                {
                    rsa.FromXmlString(storedPublicKey);
                    byte[] messageBytes = Encoding.ASCII.GetBytes(message);
                    // Verify using SHA1 as the hash algorithm
                    bool verified = rsa.VerifyData(messageBytes, "SHA1", signatureBytes);
                    if (verified)
                    {
                        Console.WriteLine("Message was successfully signed");
                    }
                    else
                    {
                        Console.WriteLine("Message was not successfully signed");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred during signature verification: " + ex.Message);
                }
            }
        }
        catch (HttpRequestException)
        {
            Console.WriteLine("Message was not successfully signed");
        }
    }

    // Converts a hex string with dashes
    private static byte[] HexStringToByteArray(string hex)
    {
        string[] hexValues = hex.Split('-');
        byte[] bytes = new byte[hexValues.Length];
        for (int i = 0; i < hexValues.Length; i++)
        {
            bytes[i] = Convert.ToByte(hexValues[i], 16);
        }
        return bytes;
    }
    #endregion

    // Helper method to print responses uniformly
    private static async Task PrintResponse(HttpResponseMessage response)
    {
        string responseContent = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine(responseContent);
        }
        else
        {
            Console.WriteLine("Error: " + responseContent);
        }
    }
}
