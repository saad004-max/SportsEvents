public class RegisterDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    // This is crucial for the Role selection (User, Organizer, Admin)
    public string Role { get; set; } = "User";
}