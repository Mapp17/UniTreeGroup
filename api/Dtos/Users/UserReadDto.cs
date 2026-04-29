public class UserReadDto
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public decimal WalletBalance { get; set; } // Just the value, not the whole object
}