namespace Hiper.Domain.Exceptions;

public abstract class DomainException : Exception
{
    public string Code { get; }
    public Dictionary<string, string[]> Errors { get; }

    protected DomainException(string code, string message) : base(message)
    {
        Code = code;
        Errors = new Dictionary<string, string[]>();
    }

    protected DomainException(string code, string message, Dictionary<string, string[]> errors) : base(message)
    {
        Code = code;
        Errors = errors;
    }
}

public class ValidationException : DomainException
{
    public ValidationException(string message)
        : base("VALIDATION_ERROR", message)
    {
    }

    public ValidationException(Dictionary<string, string[]> errors)
        : base("VALIDATION_ERROR", "Um ou mais erros de validação ocorreram", errors)
    {
    }
}

public class NotFoundException : DomainException
{
    public NotFoundException(string entityName, string entityId)
        : base("NOT_FOUND", $"{entityName} com ID '{entityId}' não foi encontrado")
    {
    }
}

public class BusinessRuleException : DomainException
{
    public BusinessRuleException(string message)
        : base("BUSINESS_RULE_VIOLATION", message)
    {
    }
}
