public class UserAccount
{
    public int Id { get; set; }
    public string Username { get; set; }

    public string Password { get; set; }
    public string Role { get; set; }
    public string? ActiveSessionId { get; set; }
    public DateTime? LastActivity { get; set; }
}