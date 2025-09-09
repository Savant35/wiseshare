using System.Xml.Serialization;
using Wiseshare.Domain.Common.Models;
using Wiseshare.Domain.UserAggregate.ValueObjects;

namespace Wiseshare.Domain.UserAggregate; //group user.cs into a category, userAggregate keeps related code together

public sealed class User : AggregateRoot<UserId, Guid>
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Email { get; private set; }
    public string Phone { get; private set; }
    public string Password { get; private set; }
    public string Role { get; private set; }
    public string SecurityQuestion { get; private set; }
    public string SecurityAnswer { get; private set; }
    public int FailedLoginAttempts { get; private set; } = 0;
    public bool IsActive { get; private set; } = true;

    //stripe connect id
    public string? StripeAccountId { get; private set; }

    //todo remove is true done for testing only
    public bool IsEmailVerified { get; private set; } = true;
    public DateTime? EmailVerifiedDate { get; private set; } = DateTime.UtcNow;

    //timstamp for tracking when the user was created and last updated
    public DateTime CreatedDateTime { get; private set; }
    public DateTime UpdatedDateTime { get; private set; }



    //private constructor to initialize a new user instance with given information
    private User(string firstName, string lastName, string email, string phone, string password, string securityQuestion,
                 string securityAnswer, UserId? userId = null)
    // The base constructor is called with the provided userId or a newly generated unique ID if userId is null
        : base(userId ?? UserId.CreateUnique())
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Phone = phone;
        Password = password;
        SecurityQuestion = securityQuestion;
        SecurityAnswer = securityAnswer;
        Role = "User";
    }
    //function to create user
    public static User Create(string firstName, string lastName, string email, string phone, string password,
                              string securityQuestion, string securityAnswer)
    {
        return new User(firstName, lastName, email, phone, password, securityQuestion, securityAnswer);

    }

    //function to update fields
    public void Update(string? email, string? phone, string? password, string? role, string? securityQuestion,
        string? securityAnswer ){
        if (!string.IsNullOrWhiteSpace(email))
            Email = email;
        if (!string.IsNullOrWhiteSpace(phone))
            Phone = phone;
        if (!string.IsNullOrWhiteSpace(password))
            Password = password;
        if (!string.IsNullOrWhiteSpace(role))
            Role = role;

        if (!string.IsNullOrWhiteSpace(securityQuestion))
            SecurityQuestion = securityQuestion;
        if (!string.IsNullOrWhiteSpace(securityAnswer))
            SecurityAnswer = securityAnswer;
        
        UpdatedDateTime = DateTime.UtcNow.ToLocalTime();
    }
    //tracks failed login attempts deactivates account after 5 failed login attempts
    public void RegisterFailedLogin(){
        FailedLoginAttempts++;

        if (FailedLoginAttempts >= 5) IsActive = false;
    }

    public void ResetLoginAttempts(){
        FailedLoginAttempts = 0;
    }

    //function to deactivate or activate account
    public void AccountStatus(bool active){
        IsActive = active;
        DateTime.UtcNow.ToLocalTime();
    }

    public void ResetPassword(string password)
    {
        Password = password;
    }

    public void ConfirmEmail(){
        if (IsEmailVerified) return;
        IsEmailVerified = true;
        EmailVerifiedDate = DateTime.UtcNow;
        UpdatedDateTime = DateTime.UtcNow;
    }

     //called when a new stripe express account is created
     public void SetStripeAccountId(string stripeAccountId){
        StripeAccountId = stripeAccountId;
        UpdatedDateTime = DateTime.UtcNow.ToLocalTime();
    }

#pragma warning disable CS8618 //disable warning for non-nullable since i have a nullable value in constructor
    private User()
    {
    }
#pragma warning restore CS8618
}




