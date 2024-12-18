using FluentResults;
using Microsoft.EntityFrameworkCore;
using Wiseshare.Application.Repository;
using Wiseshare.Domain.UserAggregate;
using Wiseshare.Domain.UserAggregate.ValueObjects;

namespace WiseShare.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly WiseShareDbContext _dbContext;

    public UserRepository(WiseShareDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // Add a new user
    public Result Insert(User user)
    {
        _dbContext.Users.Add(user);
        _dbContext.SaveChanges();
        return Result.Ok();
    }

    // Get a user by their email
    public Result<User> GetUserByEmail(string email)
    {
        var user = _dbContext.Users.SingleOrDefault(u => u.Email == email);
        return user is not null
            ? Result.Ok(user)
            : Result.Fail<User>("User not found.");
    }

    // Get a user by their phone
    public Result<User> GetUserByPhone(string phone)
    {
        var user = _dbContext.Users.SingleOrDefault(u => u.Phone == phone);
        return user is not null
            ? Result.Ok(user)
            : Result.Fail<User>("User not found.");
    }

    // Get a user by their unique identifier
    public Result<User> GetUserById(UserId userId)
    {
        var user = _dbContext.Users.SingleOrDefault(u => u.Id == userId);
        return user is not null
            ? Result.Ok(user)
            : Result.Fail<User>("User not found.");
    }

    // Remove a user by their unique identifier
    public Result Delete(UserId userId)
    {
        var user = _dbContext.Users.SingleOrDefault(u => u.Id == userId);
        if (user is null) return Result.Fail("User not found.");

        _dbContext.Users.Remove(user);
        _dbContext.SaveChanges();
        return Result.Ok();
    }

    // Retrieve all users
    public Result<IEnumerable<User>> GetUsers()
    {
        var users = _dbContext.Users.ToList();
        return Result.Ok(users.AsEnumerable());
    }

    // Update an existing user
    public Result Update(User user)
    {
        var existingUser = _dbContext.Users.SingleOrDefault(u => u.Id == user.Id);
        if (existingUser is null) return Result.Fail("User not found.");

        // Manually update fields
        _dbContext.SaveChanges();
        return Result.Ok();
    }

    // Save changes to the database
    public Result Save()
    {
        _dbContext.SaveChanges();
        return Result.Ok();
    }
}