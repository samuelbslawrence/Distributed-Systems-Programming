using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DistSysAcwServer.Repositories;
using System.Linq;
using System;

namespace DistSysAcwServer.Controllers
{
    public partial class ProtectedController : ControllerBase
    {
        // Static RSA provider for signing and decryption across all requests
        private static RSACryptoServiceProvider _rsaProvider = new RSACryptoServiceProvider();
    }

    #region Task 9 & 13
    [Route("api/protected")]
    [ApiController]
    [Authorize(Roles = "User,Admin")] // Only authenticated users with 'User' or 'Admin' roles
    public partial class ProtectedController : ControllerBase
    {
        private readonly UserRepository _userRepository;

        // Constructor injection of UserRepository for logging
        public ProtectedController(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet("hello")]
        public async Task<IActionResult> Hello()
        {
            // Log the request if ApiKey header is present
            if (Request.Headers.TryGetValue("ApiKey", out var apiKeyValues))
            {
                string apiKey = apiKeyValues.First();
                await _userRepository.AddLogAsync(apiKey, "User requested /Protected/Hello");
            }

            // Get the logged-in user's name and return greeting
            var userName = User.Identity?.Name;
            if (string.IsNullOrEmpty(userName))
            {
                return BadRequest("Bad Request"); // Should not happen if authenticated
            }
            return Ok($"Hello {userName}");
        }

        [HttpGet("SHA1")]
        public async Task<IActionResult> SHA1Hash([FromQuery] string message)
        {
            // Log the request for SHA1 endpoint
            if (Request.Headers.TryGetValue("ApiKey", out var apiKeyValues))
            {
                string apiKey = apiKeyValues.First();
                await _userRepository.AddLogAsync(apiKey, "User requested /Protected/SHA1");
            }

            // Validate the input message
            if (string.IsNullOrWhiteSpace(message))
            {
                return BadRequest("Bad Request");
            }

            // Compute SHA1 hash and return uppercase hex string
            using (var sha1 = SHA1.Create())
            {
                byte[] hashBytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(message));
                StringBuilder sb = new StringBuilder(hashBytes.Length * 2);
                foreach (var b in hashBytes)
                {
                    sb.Append(b.ToString("X2"));
                }
                return Ok(sb.ToString());
            }
        }

        [HttpGet("SHA256")]
        public async Task<IActionResult> ComputeSHA256([FromQuery] string message)
        {
            // Log the request for SHA256 endpoint
            if (Request.Headers.TryGetValue("ApiKey", out var apiKeyValues))
            {
                string apiKey = apiKeyValues.First();
                await _userRepository.AddLogAsync(apiKey, "User requested /Protected/SHA256");
            }

            // Validate the input message
            if (string.IsNullOrWhiteSpace(message))
            {
                return BadRequest("Bad Request");
            }

            // Compute SHA256 hash and return uppercase hex string
            using (var sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(message));
                StringBuilder sb = new StringBuilder(hashBytes.Length * 2);
                foreach (var b in hashBytes)
                {
                    sb.Append(b.ToString("X2"));
                }
                return Ok(sb.ToString());
            }
        }
    }
    #endregion

    #region Task 11
    public partial class ProtectedController : ControllerBase
    {
        [HttpGet("GetPublicKey")]
        public async Task<IActionResult> GetPublicKey()
        {
            // Log the request if ApiKey header is present
            if (Request.Headers.TryGetValue("ApiKey", out var apiKeyValues))
            {
                string apiKey = apiKeyValues.First();
                await _userRepository.AddLogAsync(apiKey, "User requested /Protected/GetPublicKey");
            }

            // Return the public key XML without private parameters
            string publicKeyXml = _rsaProvider.ToXmlString(false);
            return Ok(publicKeyXml);
        }
    }
    #endregion

    #region Task 12
    public partial class ProtectedController : ControllerBase
    {
        [HttpGet("Sign")]
        public async Task<IActionResult> Sign([FromQuery] string message)
        {
            // Validate the input message
            if (string.IsNullOrWhiteSpace(message))
            {
                return BadRequest("Bad Request");
            }

            // Log the request for Sign endpoint
            if (Request.Headers.TryGetValue("ApiKey", out var apiKeyValues))
            {
                string apiKey = apiKeyValues.First();
                await _userRepository.AddLogAsync(apiKey, "User requested /Protected/Sign");
            }

            try
            {
                // Sign the message bytes using SHA1
                byte[] messageBytes = Encoding.ASCII.GetBytes(message);
                byte[] signatureBytes = _rsaProvider.SignData(messageBytes, CryptoConfig.MapNameToOID("SHA1"));

                // Convert signature bytes to dash-separated hex string
                string signatureHex = string.Join("-", signatureBytes.Select(b => b.ToString("X2")));
                return Ok(signatureHex);
            }
            catch
            {
                return BadRequest("Bad Request");
            }
        }
    }
    #endregion

    #region Task 14
    public partial class ProtectedController : ControllerBase
    {
        public class MashifyRequest
        {
            public string EncryptedString { get; set; }
            public string EncryptedSymKey { get; set; }
            public string EncryptedIV { get; set; }
        }

        [HttpGet("Mashify")]
        public async Task<IActionResult> Mashify([FromBody] MashifyRequest request)
        {
            // Log the request (do not log encrypted data)
            if (Request.Headers.TryGetValue("ApiKey", out var apiKeyValues))
            {
                string apiKey = apiKeyValues.First();
                await _userRepository.AddLogAsync(apiKey, "User requested /Protected/Mashify");
            }

            // Validate encrypted inputs
            if (string.IsNullOrWhiteSpace(request.EncryptedString) ||
                string.IsNullOrWhiteSpace(request.EncryptedSymKey) ||
                string.IsNullOrWhiteSpace(request.EncryptedIV))
            {
                return BadRequest("Bad Request");
            }

            try
            {
                // Convert dash-separated hex strings to byte arrays
                byte[] encryptedMessageBytes = HexStringToByteArray(request.EncryptedString);
                byte[] encryptedSymKeyBytes = HexStringToByteArray(request.EncryptedSymKey);
                byte[] encryptedIVBytes = HexStringToByteArray(request.EncryptedIV);

                // Decrypt with RSA private key using OAEP SHA1 padding
                byte[] decryptedMessageBytes = _rsaProvider.Decrypt(encryptedMessageBytes, RSAEncryptionPadding.OaepSHA1);
                byte[] decryptedSymKeyBytes = _rsaProvider.Decrypt(encryptedSymKeyBytes, RSAEncryptionPadding.OaepSHA1);
                byte[] decryptedIVBytes = _rsaProvider.Decrypt(encryptedIVBytes, RSAEncryptionPadding.OaepSHA1);

                // Convert decrypted bytes to string and mashify
                string decryptedMessage = Encoding.UTF8.GetString(decryptedMessageBytes);
                string mashified = Mashify(decryptedMessage);

                // Encrypt mashified text with AES using the symmetric key and IV
                byte[] mashifiedBytes = Encoding.UTF8.GetBytes(mashified);
                byte[] encryptedResultBytes;
                using (var aes = Aes.Create())
                {
                    aes.Key = decryptedSymKeyBytes;
                    aes.IV = decryptedIVBytes;
                    aes.Padding = PaddingMode.PKCS7;

                    using (var encryptor = aes.CreateEncryptor())
                    {
                        encryptedResultBytes = encryptor.TransformFinalBlock(mashifiedBytes, 0, mashifiedBytes.Length);
                    }
                }

                // Return encrypted result as dash-separated hex string
                string resultHex = ByteArrayToHexString(encryptedResultBytes);
                return Ok(resultHex);
            }
            catch
            {
                return BadRequest("Bad Request");
            }
        }

        private byte[] HexStringToByteArray(string hex)
        {
            // Convert dash-separated hex string into byte array
            return hex.Split('-').Select(s => Convert.ToByte(s, 16)).ToArray();
        }

        private string ByteArrayToHexString(byte[] bytes)
        {
            // Convert byte array to dash-separated hex string
            return string.Join("-", bytes.Select(b => b.ToString("X2")));
        }

        private string Mashify(string input)
        {
            // Replace vowels with 'X' and reverse the character array
            char[] chars = input.Select(c => "aeiouAEIOU".Contains(c) ? 'X' : c).ToArray();
            Array.Reverse(chars);
            return new string(chars);
        }
    }
    #endregion
}