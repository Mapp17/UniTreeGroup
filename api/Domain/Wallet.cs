

using System.Text.Json.Serialization;

public class Wallet : BaseModel
{
    public int UserId { get; set; }
    public decimal Balance { get; set; } = 0m;
    public string Currency { get; set; } = "ZAR";

    // Navigation
    [JsonIgnore]
    public User User { get; set; } = null!;
    public ICollection<Transactions> Transactions { get; set; } = new List<Transactions>();
}