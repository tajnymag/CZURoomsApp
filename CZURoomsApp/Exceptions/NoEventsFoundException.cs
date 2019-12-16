using System;

namespace CZURoomsApp.Exceptions
{
    /// <summary>
    /// Výjimka informující o nenalezení žádných rozvrhových akcí v zadaných podmínkách
    /// </summary>
    public class NoEventsFoundException : Exception
    {
        public NoEventsFoundException()
        {
        }

        public NoEventsFoundException(string message) : base(message)
        {
        }

        public NoEventsFoundException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}