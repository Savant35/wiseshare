using FluentResults;
using Wiseshare.Domain.UserAggregate;
using Wiseshare.Domain.UserAggregate.ValueObjects;
using Wiseshare.Application.Repository;

namespace WiseShare.Infrastructure.Repository
{
    public class InMemoryUserRepository : IUserRepository
    {
        private readonly List<User> _users = new();

        public Result<User> GetUserByEmail(string email)
        {
            var user = _users.FirstOrDefault(u => u.Email == email);
            return user is not null 
                ? Result.Ok(user) 
                : Result.Fail<User>("User not found.");
        }

        public Result<User> GetUserByPhone(string phone)
        {
            var user = _users.FirstOrDefault(u => u.Phone == phone);
            return user is not null 
                ? Result.Ok(user) 
                : Result.Fail<User>("User not found.");
        }

        public Result<User> GetUserById(UserId userId)
        {
            var user = _users.FirstOrDefault(u => u.Id == userId);
            return user is not null 
                ? Result.Ok(user) 
                : Result.Fail<User>("User not found.");
        }

        public Result<IEnumerable<User>> GetUsers()
        {
            return Result.Ok((IEnumerable<User>)_users);
        }

        public Result Insert(User user)
        {
            if (_users.Any(u => u.Email == user.Email))
            {
                return Result.Fail("User already exists.");
            }

            _users.Add(user);
            return Result.Ok();
        }

        public Result Delete(UserId userId)
        {
            var user = _users.FirstOrDefault(u => u.Id == userId);
            if (user is null)
            {
                return Result.Fail("User not found.");
            }

            _users.Remove(user);
            return Result.Ok();
        }

        public Result Update(User user)
        {
            var existingUser = _users.FirstOrDefault(u => u.Id == user.Id);
            if (existingUser is null)
            {
                return Result.Fail("User not found.");
            }

            // Update the existing user
            //existingUser.UpdateContactInfo(user.Email, user.Phone); // Use a method in the `User` entity for updating
            return Result.Ok();
        }

        public Result Save()
        {
            // In-memory save is effectively a no-op
            return Result.Ok();
        }
    }
}
