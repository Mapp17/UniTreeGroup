
public class User : BaseModel
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    // Navigation
    public Wallet? Wallet { get; set; }
    public Membership? Memberships { get; set; } 
    public ICollection<Transactions> Transactions { get; set; } = new List<Transactions>();
}