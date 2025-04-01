namespace Backend.Models
{
public class User
{
    public int UserId { get; set; }
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Role { get; set; } = "User"; // Standardrolle
    public string DummyColumn { get; set; } = "Temp"; // Dummy-kolonne til testformål
}
}