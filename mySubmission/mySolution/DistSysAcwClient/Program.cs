using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

class Program
{
    #region Task 10
    // Test Server
    private const string BaseUrl = "http://150.237.94.9/5488600/";
    // IIS
    //private const string BaseUrl = "https://localhost:44394/";
    // Kestrel
    //private const string BaseUrl = "https://localhost:5001/";

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
                                string message = string.Join(" ", args.Skip(2));
                                await ProtectedSign(message);
                            }
                            else
                            {
                                Console.WriteLine("Please provide a message for Protected Sign.");
                            }
                        }
                        else if (subCommand == "mashify")
                        {
                            if (args.Length > 2)
                            {
                                string message = string.Join(" ", args.Skip(2));
                                await ProtectedMashify(message);
                            }
                            else
                            {
                                Console.WriteLine("Please provide a string to Mashify.");
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
            HttpResponseMessage response = await client.GetAsync($"{BaseUrl}api/talkback/hello");

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
            HttpResponseMessage response = await client.GetAsync($"{BaseUrl}api/talkback/sort?{query}");
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
            HttpResponseMessage response = await client.GetAsync($"{BaseUrl}api/user/new?username={username}");
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
            HttpResponseMessage response = await client.PostAsync($"{BaseUrl}api/user/new", content);
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
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, $"{BaseUrl}api/user/removeuser?username={storedUserName}");
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

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"{BaseUrl}api/user/changerole");
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
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{BaseUrl}api/protected/hello");
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
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{BaseUrl}api/protected/sha1?message={encodedMessage}");
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
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{BaseUrl}api/protected/sha256?message={encodedMessage}");
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
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{BaseUrl}api/protected/getpublickey");
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
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{BaseUrl}api/protected/sign?message={encodedMessage}");
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

    #region Task 14
    private static async Task ProtectedMashify(string message)
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

        var (aesKey, aesIV) = GenerateAesKeyAndIV();
        byte[] encryptedMessage = EncryptWithAes(message, aesKey, aesIV);
        byte[] encryptedKey = EncryptWithRsa(aesKey, storedPublicKey);
        byte[] encryptedIV = EncryptWithRsa(aesIV, storedPublicKey);
        byte[] encryptedString = encryptedMessage;

        var payload = new
        {
            encryptedString = BitConverter.ToString(encryptedString),
            encryptedSymKey = BitConverter.ToString(encryptedKey),
            encryptedIV = BitConverter.ToString(encryptedIV)
        };

        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        var request = new HttpRequestMessage(HttpMethod.Post, $"{BaseUrl}api/protected/mashify");
        request.Headers.Add("ApiKey", storedApiKey);
        request.Content = content;

        try
        {
            HttpResponseMessage response = await client.SendAsync(request);
            string responseHex = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                byte[] encryptedResult = HexStringToByteArray(responseHex);
                string result = DecryptWithAes(encryptedResult, aesKey, aesIV);
                Console.WriteLine(result);
            }
            else Console.WriteLine("Bad Request");
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred during Mashify: " + ex.Message);
        }
    }

    private static (byte[] key, byte[] iv) GenerateAesKeyAndIV()
    {
        using Aes aes = Aes.Create();
        aes.KeySize = 256;
        aes.GenerateKey();
        aes.GenerateIV();
        return (aes.Key, aes.IV);
    }

    private static byte[] EncryptWithAes(string plaintext, byte[] key, byte[] iv)
    {
        using Aes aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var ms = new MemoryStream();
        using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
        using (var sw = new StreamWriter(cs)) sw.Write(plaintext);

        return ms.ToArray();
    }

    private static string DecryptWithAes(byte[] ciphertext, byte[] key, byte[] iv)
    {
        using Aes aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var ms = new MemoryStream(ciphertext);
        using var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
        using var sr = new StreamReader(cs);
        return sr.ReadToEnd();
    }

    private static byte[] EncryptWithRsa(byte[] data, string publicKeyXml)
    {
        using var rsa = new RSACryptoServiceProvider();
        rsa.FromXmlString(publicKeyXml);
        return rsa.Encrypt(data, RSAEncryptionPadding.OaepSHA1);
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
