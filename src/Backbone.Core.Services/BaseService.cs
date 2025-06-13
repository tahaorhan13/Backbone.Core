using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using Backbone.Core.Models;
using Backbone.Core.Services.Logging;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Backbone.Core.Services
{
    public class BaseService
    {
        protected IConfiguration Config;
        protected IHttpContextAccessor HttpContextAccessor;
        protected ILogService LogService;

        public BaseService(
            IHttpContextAccessor accessor,
            IConfiguration config,
            ILogService logService)
        {
            Config = config;
            HttpContextAccessor = accessor;
            LogService = logService;
        }
        public string defaultSuccesMessage => "100-1001";
        public string defaultErrorMessage => "400-1001";
        private string notFoundMessage => "Mesajın açıklaması bulunamadı.";
        public string defaultNotFoundMessage => "404-1001";
        public string defaultForbiddenMessage => "403-1001";

        #region Logging
        public async Task Log(string message, string level, string source, string? stackTrace)
        {
            await LogService.Log(message, level, source, stackTrace);
        }
        #endregion

        #region Hash Methods
        public string HashPassword(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(16);

            byte[] hash = KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA512,
                iterationCount: 100_000,
                numBytesRequested: 32
            );

            var result = new byte[48];
            Buffer.BlockCopy(salt, 0, result, 0, 16);
            Buffer.BlockCopy(hash, 0, result, 16, 32);
            return Convert.ToBase64String(result);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            byte[] decoded = Convert.FromBase64String(hashedPassword);
            byte[] salt = new byte[16];
            byte[] storedHash = new byte[32];
            Buffer.BlockCopy(decoded, 0, salt, 0, 16);
            Buffer.BlockCopy(decoded, 16, storedHash, 0, 32);

            byte[] newHash = KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA512,
                iterationCount: 100_000,
                numBytesRequested: 32
            );

            return CryptographicOperations.FixedTimeEquals(storedHash, newHash);
        }
        #endregion

        #region Crypto
        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
            {
                return plainText;
            }

            using (var aes = Aes.Create())
            {
                aes.Key = Convert.FromBase64String(Config.GetSection("Encryption:Key").Value);
                aes.GenerateIV();
                var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using var ms = new MemoryStream();
                ms.Write(aes.IV, 0, aes.IV.Length);

                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                using (var sw = new StreamWriter(cs))
                {
                    sw.Write(plainText);
                }

                return Convert.ToBase64String(ms.ToArray());
            }
        }

        public string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return cipherText;

            var fullCipher = Convert.FromBase64String(cipherText);

            using var aes = Aes.Create();
            aes.Key = Convert.FromBase64String(Config.GetSection("Encryption:Key").Value);

            byte[] iv = new byte[16];
            Array.Copy(fullCipher, 0, iv, 0, iv.Length);
            aes.IV = iv;

            var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using var ms = new MemoryStream(fullCipher, iv.Length, fullCipher.Length - iv.Length);
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);
            return sr.ReadToEnd();
        }
        #endregion

        public int GetUserId()
        {
            var authHeader = HttpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                return 0;
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();
            var tokenHandler = new JwtSecurityTokenHandler();

            if (!tokenHandler.CanReadToken(token))
            {
                return 0;
            }

            var jwtToken = tokenHandler.ReadJwtToken(token);

            var userIdClaim = jwtToken.Claims.FirstOrDefault(c =>
                c.Type == ClaimTypes.NameIdentifier || c.Type == "UserId");

            if (userIdClaim == null)
            {
                return 0;
            }
            var userId = int.TryParse(userIdClaim.Value, out var id)
                ? id
                : Convert.ToInt32(Decrypt(userIdClaim.Value));

            return userId;
        }
        public string GetIpAddressString()
        {
            return HttpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
        }

        public ApiResponse<T> ApiResponse<T>(T data, HttpStatusCode statusCode, string messageKey, string language = "message_tr")
        {
            return new ApiResponse<T>()
            {
                Code = Config.GetSection(messageKey).Key,
                Status = (int)statusCode,
                Message = Config.GetSection(messageKey).GetSection(language).Value ?? notFoundMessage,
                Data = data
            };
        }
        public ApiResponse ApiResponse(HttpStatusCode statusCode, string messageKey, string language = "message_tr", string message = null)
        {
            return new ApiResponse()
            {
                Code = messageKey != null ? Config.GetSection(messageKey).Key : "400",
                Status = (int)statusCode,
                Message = messageKey != null ? Config.GetSection(messageKey).GetSection(language).Value ?? notFoundMessage : message
            };
        }
    }

}
