using FluentResults;
using Wiseshare.Domain.UserAggregate;

namespace WiseShare.Application.Authentication;
    public interface IAuthenticationService
    {
        Result Register(string firstName, string lastName, string email, string phone, string password);
        //Result Login(string email, string password); // Returns the User object on success
       Result<(string Token, string FirstName, string LastName)> Login(string email, string password);
    }
