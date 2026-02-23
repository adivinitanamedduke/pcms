namespace Core.Exceptions;

public class ValidationException(string message)
    : BaseException(message);
