using System;

namespace MicroBluer.ServerCore
{
    public interface ITryCatch
    {
        void Invoke(Action invoke, string message = "");
        T Invoke<T>(T exceptionValue, Func<T> invoke, string message = "");
        bool Show(bool defaultResult, string message, string failMessage);
        bool Show(Func<bool> invoke, string message, string failMessage);
        void Show(string message);
        T Show<T>(T defaultResult, string message);
        void Throw(string message);
        T Throw<T>(string message);
    }
}