using Flunt.Notifications;
using Flunt.Validations;

namespace JwtStore.Core.Contexts.AccountContext.UseCases.Create;

public static class Specification
{
    public static Contract<Notification> Ensure(Request request)
    => new Contract<Notification>()
        .Requires()
        .IsLowerThan(request.Name.Length, 160, "Name", "O Nome não pode ter mais que 160 caracteres")
        .IsGreaterThan(request.Name.Length, 3, "Name", "O Nome não pode ter menos que 3 caracteres")
        .IsLowerThan(request.Password.Length, 40, "Password", "A Senha não pode ter mais que 160 caracteres")
        .IsGreaterThan(request.Password.Length, 8, "Password", "A Senha não pode ter menos que 8 caracteres")
        .IsEmail(request.Email, "Email", "E-mail inválido");
}
