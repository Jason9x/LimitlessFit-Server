namespace LimitlessFit.Models.Requests.Auth;

[Serializable]
public class LoginRequest
{
    public string? Email { get; set; }
    public string? Password { get; set;  }
}