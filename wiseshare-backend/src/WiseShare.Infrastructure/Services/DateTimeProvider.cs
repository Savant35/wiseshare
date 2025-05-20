using Wiseshare.Application.Common.Interfaces.Services;

namespace Wiseshare.Infrastructure.Services;

public class DateTimeProvider : IDateTimeProvider{
    public DateTime UtcNow => DateTime.UtcNow;
}