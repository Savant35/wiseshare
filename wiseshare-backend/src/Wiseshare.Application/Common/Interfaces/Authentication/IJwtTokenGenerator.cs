using Wiseshare.Domain.UserAggregate;
using Wiseshare.Domain.UserAggregate.ValueObjects;

namespace Wiseshare.Application.Common.Interfaces.Authentication;


public interface IJwtTokenGenerator{
    string GenerateToken(User user);
}
