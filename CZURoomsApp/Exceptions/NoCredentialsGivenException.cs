using System;

namespace CZURoomsApp.Exceptions
{
    /// <summary>
    /// Výjimka informující o nezadání přihlašovacích údajů
    /// </summary>
    public class NoCredentialsGivenException : Exception
    {
        public NoCredentialsGivenException()
        {
        }

        public NoCredentialsGivenException(string message) : base(message)
        {
        }

        public NoCredentialsGivenException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}