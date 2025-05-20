using Microsoft.AspNetCore.DataProtection;

namespace Wiseshare.Application.Common.Email;
    public class EmailVerificationService : IEmailVerificationService{
        private const string Purpose = "EmailVerification";
        private readonly IDataProtector _protector;

        public EmailVerificationService(IDataProtectionProvider provider){
            _protector = provider.CreateProtector(Purpose);
        }

        public string CreateToken(Guid userId, TimeSpan validFor){
            var expires  = DateTime.UtcNow.Add(validFor).ToString("O");
            var payload  = $"{userId}|{expires}";
            return _protector.Protect(payload);
        }

        public bool TryValidateToken(string token, out Guid userId){
            userId = Guid.Empty;
            try
            {
                var unprotected = _protector.Unprotect(token);
                var parts       = unprotected.Split('|');
                if (parts.Length != 2) return false;

                userId = Guid.Parse(parts[0]);
                var expires = DateTime.Parse(parts[1], null,
                    System.Globalization.DateTimeStyles.RoundtripKind);

                return DateTime.UtcNow <= expires;
            }
            catch
            {
                return false;
            }
        }
    }
