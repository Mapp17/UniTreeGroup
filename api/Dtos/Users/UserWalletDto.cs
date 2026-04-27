public class UserWalletDto
{
    public int UserId { get; set; }
    public string FullNames {get; set;} = string.Empty;
    public decimal Balance { get; set; } = 0m;
    public string Currency { get; set; } = "ZAR";
}