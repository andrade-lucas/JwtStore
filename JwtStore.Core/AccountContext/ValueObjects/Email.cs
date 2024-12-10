using JwtStore.Core.SharedContext.Extensions;
using JwtStore.Core.SharedContext.ValueObjects;
using System.Text.RegularExpressions;

namespace JwtStore.Core.AccountContext.ValueObjects;

public partial class Email : ValueObject
{
    private const string Pattern = @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$";

    public Email(string address)
    {
        if (string.IsNullOrEmpty(address))
            throw new Exception("E-mail inválido");

        Address = address.Trim().ToLower();

        if (Address.Length < 5)
            throw new Exception("Email-inválido");

        if (!EmailRegex().IsMatch(Address))
            throw new Exception("E-mail inválido");
    }

    public string Address { get; }
    public string Hash => Address.ToBase64();

    public static implicit operator string(Email email) => email.ToString();

    public static implicit operator Email(string address) => new Email(address);

    public override string ToString() => Address.Trim().ToLower();

    [GeneratedRegex(Pattern)]
    private static partial Regex EmailRegex();
}
