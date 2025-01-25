using JwtStore.Core.AccountContext.ValueObjects;
using JwtStore.Core.Contexts.AccountContext.Entities;
using JwtStore.Core.Contexts.AccountContext.UseCases.Create.Contracts;
using JwtStore.Core.Contexts.AccountContext.ValueObjects;
using MediatR;

namespace JwtStore.Core.Contexts.AccountContext.UseCases.Create;

public class Handler : IRequestHandler<Request, Response>
{
    private readonly IRepository _repository;
    private readonly IService _service;

    public Handler(IRepository repository, IService service)
    {
        _repository = repository;
        _service = service;
    }

    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        #region 1. Valida a requisição

        try
        {
            var res = Specification.Ensure(request);
            if (!res.IsValid)
                return new Response("Requisição inválida", 400, res.Notifications);
        }
        catch (Exception ex)
        {
            return new Response("Não foi possível validar a sua requisição", 500);
        }

        #endregion

        #region 2. Gera os objetos

        Email email;
        Password password;
        User user;

        try
        {
            email = new Email(request.Email);
            password = new Password(request.Password);
            user = new User(request.Name, email, password);
        }
        catch (Exception ex)
        {
            return new Response(ex.Message, 400);
        }

        #endregion

        #region 3. Verificar se o usuário existe

        try
        {
            var exists = await _repository.AnyAsync(request.Email, cancellationToken);
            if (exists)
                return new Response("Este e-mail já está em uso", 400);
        }
        catch
        {
            return new Response("Falha ao verificar e-mail cadastrado", 500);
        }

        #endregion

        #region 4. Persistir os dados

        try
        {
            await _repository.SaveAsync(user, cancellationToken);
        }
        catch
        {
            return new Response("Falha ao persistir usuário", 500);
        }

        #endregion

        #region 5. Enviar e-mail de boas ativação

        try
        {
            await _service.SendVerificationEmailAsync(user, cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        #endregion

        return new Response("Conta criada", new ResponseData(user.Id, user.Name, user.Email));
    }
}
