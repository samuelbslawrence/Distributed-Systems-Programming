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
        // Static RSA provider to hold the key pair across all requests.
        // This instance is generated once and used for signing and decryption.
        private static RSACryptoServiceProvider _rsaProvider = new RSACryptoServiceProvider();
    }

    #region Task 9 & 13
    [Route("api/protected")]
    [ApiController]
    [Authorize(Roles = "User,Admin")]
    public partial class ProtectedController : ControllerBase
    {
        private readonly UserRepository _userRepository;

        // Inject UserRepository via constructor
        public ProtectedController(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // GET: api/Protected/Hello
        [HttpGet("hello")]
        public async Task<IActionResult> Hello()
        {
            if (Request.Headers.TryGetValue("ApiKey", out var apiKeyValues))
            {
                string apiKey = apiKeyValues.First();
                await _userRepository.AddLogAsync(apiKey, "User requested /Protected/Hello");
            }

            var userName = User.Identity?.Name;
            if (string.IsNullOrEmpty(userName))
            {
                return BadRequest("Bad Request");
            }
            return Ok($"Hello {userName}");
        }

        // GET: api/Protected/SHA1?message=Hello
        [HttpGet("SHA1")]
        public async Task<IActionResult> SHA1Hash([FromQuery] string message)
        {
            if (Request.Headers.TryGetValue("ApiKey", out var apiKeyValues))
            {
                string apiKey = apiKeyValues.First();
                await _userRepository.AddLogAsync(apiKey, "User requested /Protected/SHA1");
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                return BadRequest("Bad Request");
            }

            using (var sha1 = System.Security.Cryptography.SHA1.Create())
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

        // GET: api/Protected/SHA256?message=hello
        [HttpGet("SHA256")]
        public async Task<IActionResult> ComputeSHA256([FromQuery] string message)
        {
            // Task 13: Log the request for SHA256 endpoint
            if (Request.Headers.TryGetValue("ApiKey", out var apiKeyValues))
            {
                string apiKey = apiKeyValues.First();
                await _userRepository.AddLogAsync(apiKey, "User requested /Protected/SHA256");
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                return BadRequest("Bad Request");
            }

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
        // GET: api/Protected/GetPublicKey
        [HttpGet("GetPublicKey")]
        public async Task<IActionResult> GetPublicKey()
        {
            // Log the request for GetPublicKey endpoint
            if (Request.Headers.TryGetValue("ApiKey", out var apiKeyValues))
            {
                string apiKey = apiKeyValues.First();
                await _userRepository.AddLogAsync(apiKey, "User requested /Protected/GetPublicKey");
            }
            // Return the public key portion (XML string) of the RSA key pair.
            string publicKeyXml = _rsaProvider.ToXmlString(false);
            return Ok(publicKeyXml);
        }
    }
    #endregion

    #region Task 12
    public partial class ProtectedController : ControllerBase
    {
        // GET: api/Protected/Sign?message=Hello
        [HttpGet("Sign")]
        public async Task<IActionResult> Sign([FromQuery] string message)
        {
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
                byte[] messageBytes = Encoding.ASCII.GetBytes(message);
                // Sign the data using RSA with SHA1
                byte[] signatureBytes = _rsaProvider.SignData(messageBytes, CryptoConfig.MapNameToOID("SHA1"));
                // Convert to hex string with dashes as delimiters
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
        // Model representing the expected JSON object in the request body for Mashify endpoint
        public class MashifyRequest
        {
            public string EncryptedString { get; set; }
            public string EncryptedSymKey { get; set; }
            public string EncryptedIV { get; set; }
        }

        // POST: api/Protected/Mashify
        // This endpoint requires Admin privileges.
        [HttpPost("Mashify")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Mashify([FromBody] MashifyRequest request)
        {
            // Log the request for Mashify endpoint (do not log sensitive encrypted data)
            if (Request.Headers.TryGetValue("ApiKey", out var apiKeyValues))
            {
                string apiKey = apiKeyValues.First();
                await _userRepository.AddLogAsync(apiKey, "User requested /Protected/Mashify");
            }

            if (string.IsNullOrWhiteSpace(request.EncryptedString) ||
                string.IsNullOrWhiteSpace(request.EncryptedSymKey) ||
                string.IsNullOrWhiteSpace(request.EncryptedIV))
            {
                return BadRequest("Bad Request");
            }

            try
            {
                // Use the static RSA provider for decryption (private key)
                byte[] encryptedMessageBytes = HexStringToByteArray(request.EncryptedString);
                byte[] encryptedSymKeyBytes = HexStringToByteArray(request.EncryptedSymKey);
                byte[] encryptedIVBytes = HexStringToByteArray(request.EncryptedIV);

                // Decrypt using RSA with OaepSHA1 padding
                byte[] decryptedMessageBytes = _rsaProvider.Decrypt(encryptedMessageBytes, RSAEncryptionPadding.OaepSHA1);
                byte[] decryptedSymKeyBytes = _rsaProvider.Decrypt(encryptedSymKeyBytes, RSAEncryptionPadding.OaepSHA1);
                byte[] decryptedIVBytes = _rsaProvider.Decrypt(encryptedIVBytes, RSAEncryptionPadding.OaepSHA1);

                // Convert decrypted message to string
                string decryptedMessage = Encoding.UTF8.GetString(decryptedMessageBytes);

                // Mashify the string
                string mashified = Mashify(decryptedMessage);

                // Encrypt the mashified string using AES with the decrypted symmetric key and IV
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

                // Convert the encrypted result to hex string with dashes as delimiters
                string resultHex = ByteArrayToHexString(encryptedResultBytes);
                return Ok(resultHex);
            }
            catch
            {
                // Return "Bad Request" with status code 400 if an error occurs
                return BadRequest("Bad Request");
            }
        }

        // Helper method to convert a hex string with dashes to a byte array
        private byte[] HexStringToByteArray(string hex)
        {
            return hex.Split('-').Select(s => Convert.ToByte(s, 16)).ToArray();
        }

        // Helper method to convert a byte array to a hex string with dashes as delimiters
        private string ByteArrayToHexString(byte[] bytes)
        {
            return string.Join("-", bytes.Select(b => b.ToString("X2")));
        }

        // Helper method to mashify a string:
        // 1. Replace all vowels with uppercase 'X'
        // 2. Reverse the string
        private string Mashify(string input)
        {
            char[] chars = input.Select(c => "aeiouAEIOU".Contains(c) ? 'X' : c).ToArray();
            Array.Reverse(chars);
            return new string(chars);
        }
    }
    #endregion
}