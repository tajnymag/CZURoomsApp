using System;

namespace CZURoomsApp.Exceptions
{
    /// <summary>
    /// Výjimka informující o jakékoli chybě vzniklé při přihlašování
    /// </summary>
    public class LoginErrorException : Exception
    {
        public LoginErrorException()
        {
        }

        public LoginErrorException(string message) : base(message)
        {
        }

        public LoginErrorException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}