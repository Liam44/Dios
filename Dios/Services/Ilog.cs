using System;

namespace Dios.Services
{
    public interface ILog<T>
    {
        void LogError(Exception ex, string message, params object[] args);
        void LogInformation(string message, params object[] args);
        void LogWarning(string message, params object[] args);
    }
}
