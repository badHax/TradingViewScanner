namespace TVScanner.Shared.Logging
{
    public interface IAbstractLogger
    {
        void LogInformation<T>(T classType, string message);
        void LogError<T>(T classType, string message);
    }
}
