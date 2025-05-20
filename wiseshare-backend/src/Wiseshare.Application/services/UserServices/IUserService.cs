
using FluentResults;
using Wiseshare.Domain.UserAggregate;
using Wiseshare.Domain.UserAggregate.ValueObjects;

namespace Wiseshare.Application.services.UserServices;

public interface IUserService{

        Task<Result<User>> GetUserByIdAsync(UserId userId);
        Task<Result<User>> GetUserByEmailAsync(string userEmail);
        Task<Result<User>> GetUserByPhoneAsync(string userPhone);
        Task<Result> SaveAsync();
        Task<Result<IEnumerable<User>>> GetUsersAsync();
        Task<Result> InsertAsync(User user);
        Task<Result> UpdateAsync(User user);
        Task<Result> DeactivateUserAsync(UserId userId);
        Task<Result> ReactivateUserAsync(UserId userId);
        Task<Result> DeleteAsync(UserId userId);
}