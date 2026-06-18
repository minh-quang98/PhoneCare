using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using PhoneCare_API.Models.DTO;

namespace PhoneCare_API.Services
{
    public class AuthTokenService
    {
        private readonly IConfiguration _configuration;

        public AuthTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public (string Token, DateTime ExpiresAt) CreateToken(CurrentUserDto user)
        {
            var expiresAt = DateTime.UtcNow.AddHours(GetExpirationHours());
            var payload = JsonSerializer.Serialize(new TokenPayload
            {
                UserId = user.Id,
                UserName = user.UserName,
                FullName = user.FullName,
                CoSoCuaHangId = user.CoSoCuaHangId,
                LoaiNhanVien = user.LoaiNhanVien,
                ExpiresAtUtc = expiresAt
            });

            var payloadPart = Base64UrlEncode(Encoding.UTF8.GetBytes(payload));
            var signaturePart = Sign(payloadPart);
            return ($"{payloadPart}.{signaturePart}", expiresAt.ToLocalTime());
        }

        public bool TryValidate(string? token, out CurrentUserDto user)
        {
            user = new CurrentUserDto();
            if (string.IsNullOrWhiteSpace(token)) return false;

            var parts = token.Split('.');
            if (parts.Length != 2) return false;

            var expectedSignature = Sign(parts[0]);
            if (!FixedTimeEquals(expectedSignature, parts[1])) return false;

            try
            {
                var json = Encoding.UTF8.GetString(Base64UrlDecode(parts[0]));
                var payload = JsonSerializer.Deserialize<TokenPayload>(json);
                if (payload == null || payload.ExpiresAtUtc <= DateTime.UtcNow) return false;

                user = new CurrentUserDto
                {
                    Id = payload.UserId,
                    UserName = payload.UserName,
                    FullName = payload.FullName,
                    CoSoCuaHangId = payload.CoSoCuaHangId,
                    LoaiNhanVien = payload.LoaiNhanVien
                };
                return true;
            }
            catch
            {
                return false;
            }
        }

        private int GetExpirationHours()
        {
            return int.TryParse(_configuration["Auth:ExpirationHours"], out var hours) && hours > 0 ? hours : 8;
        }

        private string Sign(string payloadPart)
        {
            var secret = _configuration["Auth:Secret"];
            if (string.IsNullOrWhiteSpace(secret))
            {
                secret = "PhoneCare_API_Default_Development_Secret_Change_Me";
            }

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
            return Base64UrlEncode(hmac.ComputeHash(Encoding.UTF8.GetBytes(payloadPart)));
        }

        private static bool FixedTimeEquals(string left, string right)
        {
            var leftBytes = Encoding.UTF8.GetBytes(left);
            var rightBytes = Encoding.UTF8.GetBytes(right);
            return leftBytes.Length == rightBytes.Length
                && CryptographicOperations.FixedTimeEquals(leftBytes, rightBytes);
        }

        private static string Base64UrlEncode(byte[] bytes)
        {
            return Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        }

        private static byte[] Base64UrlDecode(string value)
        {
            var padded = value.Replace('-', '+').Replace('_', '/');
            switch (padded.Length % 4)
            {
                case 2:
                    padded += "==";
                    break;
                case 3:
                    padded += "=";
                    break;
            }

            return Convert.FromBase64String(padded);
        }

        private class TokenPayload
        {
            public int UserId { get; set; }
            public string UserName { get; set; } = string.Empty;
            public string FullName { get; set; } = string.Empty;
            public int CoSoCuaHangId { get; set; }
            public string LoaiNhanVien { get; set; } = string.Empty;
            public DateTime ExpiresAtUtc { get; set; }
        }
    }
}
