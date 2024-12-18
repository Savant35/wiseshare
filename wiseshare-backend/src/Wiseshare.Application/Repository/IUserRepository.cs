using FluentResults;
using Wiseshare.Domain.UserAggregate;
using Wiseshare.Domain.UserAggregate.ValueObjects;
namespace Wiseshare.Application.Repository;
public interface IUserRepository
{
    public Result<User> GetUserByEmail(string UserEmail);
    public Result<User> GetUserByPhone(string UserPhone);
    public Result<User> GetUserById(UserId userId);
    public Result<IEnumerable<User>> GetUsers();
    public Result Insert(User user);
    public Result Delete(UserId userId);
    public Result Update(User user);
    public Result Save();
}