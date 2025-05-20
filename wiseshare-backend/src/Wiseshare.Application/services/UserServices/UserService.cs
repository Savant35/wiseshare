using System.Net.Mail;
using FluentResults;
using Wiseshare.Application.Repository;
using Wiseshare.Domain.UserAggregate;
using Wiseshare.Domain.UserAggregate.ValueObjects;

namespace Wiseshare.Application.services.UserServices;


public class UserService : IUserService{
    private readonly IUserRepository _userRepository;
    public UserService(IUserRepository userRepository){
        _userRepository = userRepository;
    }

    public async Task<Result<User>> GetUserByIdAsync(UserId userId){
            return await _userRepository.GetUserByIdAsync(userId);
        }

    public async Task<Result<IEnumerable<User>>> GetUsersAsync(){
        return await _userRepository.GetUsersAsync();
    }

    public async Task<Result<User>> GetUserByEmailAsync(string email){
        return await _userRepository.GetUserByEmailAsync(email);
    }

    public async Task<Result<User>> GetUserByPhoneAsync(string phone){
        return await _userRepository.GetUserByPhoneAsync(phone);
    }

    public async Task<Result> InsertAsync(User user){

        // Validate email format
        try{
            var addr = new MailAddress(user.Email);
            if (addr.Address != user.Email){
                return Result.Fail("The provided email address is invalid.");
            }
        }catch{//catches the error when MailAdress throws an exception for wrongly formatted emails
            return Result.Fail("The provided email address is invalid.");
        }

         return await _userRepository.InsertAsync(user);
    }

    public async Task<Result> SaveAsync(){
            return await _userRepository.SaveAsync();
        }
    
        public async Task<Result> DeleteAsync(UserId userId){
            return await _userRepository.DeleteAsync(userId);
        }

    public async Task<Result> UpdateAsync(User updatedUser){

        if (updatedUser is null) return Result.Fail("No updated user data was provided.");

        // Retrieve the current user from the database by ID
        var existingUserResult = await _userRepository.GetUserByIdAsync(UserId.Create(updatedUser.Id.Value));
        if (existingUserResult.IsFailed) return Result.Fail("Invalid user ID.");

        var existingUser = existingUserResult.Value;

        // Validate role to make sure on admin or user is accepted 
        if (!string.IsNullOrWhiteSpace(updatedUser.Role)
            && !string.Equals(updatedUser.Role, "Admin", StringComparison.OrdinalIgnoreCase)
            && !string.Equals(updatedUser.Role, "User", StringComparison.OrdinalIgnoreCase))
        {
            return Result.Fail("Invalid role provided. Role must be 'Admin' or 'User'.");
        }

        //update method from domaon to update the user
        existingUser.Update( updatedUser.Email,updatedUser.Phone, updatedUser.Password, updatedUser.Role,
                             updatedUser.SecurityQuestion,updatedUser.SecurityAnswer);

        var updateResult = await _userRepository.UpdateAsync(existingUser);
        if (updateResult.IsFailed) return Result.Fail(updateResult.Errors.First().Message);

        var saveResult = await _userRepository.SaveAsync();
        if (saveResult.IsFailed) return Result.Fail(saveResult.Errors.First().Message);

        return Result.Ok();
    }


    public async Task<Result> DeactivateUserAsync(UserId userId){

        var userResult = await _userRepository.GetUserByIdAsync(userId);

        if (userResult.IsFailed) return Result.Fail("User not found.");

        var user = userResult.Value;
        user.AccountStatus(false); // Sets IsActive to false and updates UpdatedDateTime

        var updateResult = await _userRepository.UpdateAsync(user);
        if (updateResult.IsFailed) return updateResult;

        var saveResult = await _userRepository.SaveAsync();
        if (saveResult.IsFailed) return Result.Fail(saveResult.Errors.First().Message);

        return Result.Ok();
    }

    public async Task<Result> ReactivateUserAsync(UserId userId){

        var userResult = await _userRepository.GetUserByIdAsync(userId);

        if (userResult.IsFailed) return Result.Fail("User not found.");

        var user = userResult.Value;
        user.AccountStatus(true); // Sets IsActive to true and updates UpdatedDateTime
        user.ResetLoginAttempts();

        var updateResult = await _userRepository.UpdateAsync(user);
        if (updateResult.IsFailed) return updateResult;

        var saveResult = await _userRepository.SaveAsync();
        if (saveResult.IsFailed) return Result.Fail(saveResult.Errors.First().Message);

        return Result.Ok();
    }


}
