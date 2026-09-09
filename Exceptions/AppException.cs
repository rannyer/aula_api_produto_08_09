using System.Net;

namespace ProdutosApi.Exceptions;

public abstract class AppException : Exception
{
    public abstract HttpStatusCode StatusCode { get; }
    public abstract string Titulo { get; }

    protected AppException(string message) : base(message)
    {
    }
}

public class RecursoNaoEncontradoException : AppException
{
    public override HttpStatusCode StatusCode => HttpStatusCode.NotFound;
    public override string Titulo => "Recurso não encontrado";

    public RecursoNaoEncontradoException(string message) : base(message)
    {
    }

    public RecursoNaoEncontradoException(string recurso, object chave)
        : base($"O recurso '{recurso}' com a chave '{chave}' não foi encontrado.")
    {
    }
}

public class ConflitoException : AppException
{
    public override HttpStatusCode StatusCode => HttpStatusCode.Conflict;
    public override string Titulo => "Conflito de recurso";

    public ConflitoException(string message) : base(message)
    {
    }
    
}

public class RegraNegocioException : AppException
{
    public override HttpStatusCode StatusCode => HttpStatusCode.UnprocessableEntity;
    public override string Titulo => "Regra de negócio violada";

    public RegraNegocioException(string message) : base(message)
    {
    }
}