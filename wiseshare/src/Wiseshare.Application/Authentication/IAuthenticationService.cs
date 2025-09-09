using FluentResults;

namespace Wiseshare.Application.Authentication;
public interface IAuthenticationService{
    Task<Result> RegisterAsync(string firstName, string lastName, string email, string phone, string password, string securityQuestion, string securityAnswer);
    Task<Result<(string Token, string FirstName, string LastName, string Role)>> LoginAsync(string email, string password);
    Task<Result> ResetPasswordAsync(string email, string securityAnswer, string newPassword);
}
