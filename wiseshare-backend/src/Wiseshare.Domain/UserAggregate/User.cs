using System.Dynamic;
using Wiseshare.Domain.Common.Models;
using Wiseshare.Domain.UserAggregate.ValueObjects;

namespace Wiseshare.Domain.UserAggregate;//group user.cs into a category, userAggregate keeps related code together

public sealed class User: AggregateRoot<UserId, Guid> {
    public string FirstName { get; private set; }
    public string LastName {get; private set;}
    public string Email {get; private set;}
    public string Phone {get; private set;}
    public string Password {get; private set;}
    
    //timstamp for tracking when the user was created and last updated
    public DateTime CreatedDateTime {get; private set;}
    public DateTime UpdatedDateTime {get; private set;}



    //private constructor to initialize a new user instance with given information
    private User(string firstName, string lastName, string email, string phone, string password, UserId? userId = null)
    // The base constructor is called with the provided userId or a newly generated unique ID if userId is null
        : base(userId ?? UserId.CreateUnique()) 
        #region
        //userId? make userid nullable in this case if userId is null user id is null else UserId = userId
        //null
        #endregion
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Phone = phone;
            Password = password;// dont forgot to hash this
        }
                //function to create user
    public static User Create(string firstName, string lastName, string email, string phone, string password)
    {
        return new User(firstName,lastName,email,phone, password);

    }
  
        

#pragma warning disable CS8618 //disable warning for non-nullable since i have a nullable value in constructor
    private User()
    {
    }
#pragma warning restore CS8618
}




