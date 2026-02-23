namespace Core.Exceptions;

public class NotFoundException(string entityName, object key)
    : BaseException($"{entityName} with ID '{key}' was not found.");
