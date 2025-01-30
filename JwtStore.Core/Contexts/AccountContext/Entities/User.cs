using JwtStore.Core.AccountContext.ValueObjects;
using JwtStore.Core.Contexts.AccountContext.ValueObjects;
using JwtStore.Core.Contexts.SharedContext.Entities;

namespace JwtStore.Core.Contexts.AccountContext.Entities;

public class User : Entity
{
    public string Name { get; private set; } = string.Empty;
    public Email Email { get; private set; } = null!;
    public Password Password { get; private set; } = null!;
    public string Image { get; set; } = string.Empty;
    public IEnumerable<Role> Roles { get; set; } = Enumerable.Empty<Role>();

    protected User() { }

    public User(string email, string? password = null)
    {
        Email = email;
        Password = new Password(password);
    }

    public User(string name,  Email email, Password password)
    {
        Name = name;
        Email = email;
        Password = password;
    }

    public void UpdatePassword(string plainTextPassword, string code)
    {
        if (!string.Equals(code.Trim(), Password.ResetCode.Trim(), StringComparison.InvariantCultureIgnoreCase))
            throw new Exception("Código de restauração inválido");

        var password = new Password(plainTextPassword);
        Password = password;
    }

    public void UpdateEmail(Email email)
    {
        Email = email;
    }

    public void ChangePassword(string plainTextPassword)
    {
        var password = new Password(plainTextPassword);
        Password = password;
    }
}
