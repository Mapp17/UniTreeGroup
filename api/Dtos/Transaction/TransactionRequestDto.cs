public class TransactionRequestDto
{
    public int UserId { get; set; }
    public int? UniTreeGroupId { get; set; } // Added for contributions
    public decimal Amount { get; set; }
    public string Reference { get; set; } = string.Empty;
    public string? Description { get; set; }
}