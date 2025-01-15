namespace LimitlessFit.Models.Requests;

public class RegisterRequest(string name, string email, string password)
{
    public string Name { get; } = name;
    public string Email { get; } = email;
    public string Password { get; } = password;
}