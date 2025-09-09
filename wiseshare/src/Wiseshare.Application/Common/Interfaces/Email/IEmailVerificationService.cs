
namespace Wiseshare.Application.Common.Email;
    public interface IEmailVerificationService{
      
        /// Create a self‑contained token embedding userId + expiry.
        string CreateToken(Guid userId, TimeSpan validFor);

        /// Unwrap & validate; outputs the userId if valid.
        bool TryValidateToken(string token, out Guid userId);
    }
