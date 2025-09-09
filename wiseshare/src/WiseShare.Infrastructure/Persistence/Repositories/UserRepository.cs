using FluentResults;
using Wiseshare.Application.Repository;
using Wiseshare.Domain.UserAggregate;
using Wiseshare.Domain.UserAggregate.ValueObjects;
using Microsoft.EntityFrameworkCore;
using EntityFramework.Exceptions.Common;


namespace Wiseshare.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository{
    private readonly WiseshareDbContext _dbContext;

    public UserRepository(WiseshareDbContext dbContext){
        _dbContext = dbContext;
    }

    // Add a new user
    public async Task<Result> InsertAsync(User user){

        try{
            await _dbContext.Users.AddAsync(user);
            return Result.Ok();
        }
        catch (UniqueConstraintException e){
            var message = e.InnerException?.Message ?? e.Message;
            if (message.Contains("Email")){
                return Result.Fail("A user with the same Email already exists.");
            }
            else if (message.Contains("Phone")){
                return Result.Fail("A user with the same Phone number already exists.");
            }
            return Result.Fail("User Registration failed due to a database error.");
        }
    }

    // Get a user by their email
    public async Task<Result<User>> GetUserByEmailAsync(string email){
        var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Email == email);
        return user is not null ? Result.Ok(user) : Result.Fail<User>("User not found.");
    }

    // Get a user by their phone
    public async Task<Result<User>> GetUserByPhoneAsync(string phone){
        var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Phone == phone);
        return user is not null ? Result.Ok(user) : Result.Fail<User>("User not found.");
    }

    // Get a user by their unique identifier
    public async Task<Result<User>> GetUserByIdAsync(UserId userId){
        var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Id == userId);
        return user is not null ? Result.Ok(user) : Result.Fail<User>("User not found.");
    }

    // Retrieve all users
    public async Task<Result<IEnumerable<User>>> GetUsersAsync(){
        var users = await _dbContext.Users.ToListAsync();
        return Result.Ok(users.AsEnumerable());
    }

    // Update an existing user
    public async Task<Result> UpdateAsync(User user){

        var existingUser = await _dbContext.Users.SingleOrDefaultAsync(u => u.Id == user.Id);
        if (existingUser is null) return Result.Fail("User not found.");
        _dbContext.Update(user);

        return Result.Ok();
    }

    public async Task<Result> DeleteAsync(UserId userId){
        var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Id == userId);
        if (user is null) return Result.Fail("User not found.");

        _dbContext.Users.Remove(user);
        return Result.Ok();
    }

    // Save changes to the database
    public async Task<Result> SaveAsync()
        {
            try
            {
                await _dbContext.SaveChangesAsync();
                return Result.Ok();
            }
            catch (UniqueConstraintException e)
            {
                var message = e.InnerException?.Message ?? e.Message;
                if (message.Contains("Email"))
                    return Result.Fail("A user with the same Email already exists.");
                if (message.Contains("Phone"))
                    return Result.Fail("A user with the same Phone number already exists.");
                return Result.Fail("User save failed due to a database error.");
            }
            catch (Exception ex)
            {
                return Result.Fail("Database error: " + ex.Message);
            }
        }
}