using WiseShare.Application.Common.Interfaces.Services;

namespace WiseShare.Infrastructure.Services;

public class DateTimeProvider : IDateTimeProvider{
    public DateTime UtcNow => DateTime.UtcNow;
}