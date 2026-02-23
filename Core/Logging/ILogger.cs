namespace Core.Logging
{
    public interface ILogger
    {
        void Info(string message, object? source = null);

        void Warning(string message, object? source = null);

        void Error(Exception ex, object? source = null);

        void Error(string message, object? source = null);
    }
}
