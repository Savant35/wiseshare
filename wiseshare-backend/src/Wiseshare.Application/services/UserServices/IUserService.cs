
using FluentResults;
using Wiseshare.Domain.UserAggregate;
using Wiseshare.Domain.UserAggregate.ValueObjects;

namespace Wiseshare.Application.services;

public interface IUserService
{

    public Result<User> GetUserById(UserId userId);

    public Result<User> GetUserByEmail(string UserEmail);

    public Result<User> GetUserByPhone(string UserPhone);
    public Result Save();
    public Result<IEnumerable<User>> GetUsers();
    public Result Insert(User user);
    public Result Delete(UserId userId);
}