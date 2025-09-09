namespace Wiseshare.Application.Common.Payment
{
    public class PaymentFilter
    {
        public string? Status { get; set; }
        public string? Type { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public Guid? UserId { get; set; }
    }
}
