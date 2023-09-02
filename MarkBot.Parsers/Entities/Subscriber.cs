namespace MarkBot.Parsers.Entities;

public class Subscriber
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public bool ZavarFriendly { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }

    public bool HasBebraClassAuthorization => !string.IsNullOrWhiteSpace(Username) &&
                                              !string.IsNullOrWhiteSpace(Password);

    /// <inheritdoc />
    public override string ToString()
    {
        return
            $"{nameof(Id)}: {Id}, {nameof(Name)}: {Name} ({(HasBebraClassAuthorization ? "Authorized" : "Not authorized")})";
    }
}
