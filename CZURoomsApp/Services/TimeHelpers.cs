using System;
using System.Collections.Generic;

namespace CZURoomsApp.Services
{
    public class TimeHelpers
    {
        public static DateTime ExtractTimeOnly(DateTime dateTime)
        {
            return default(DateTime).Add(dateTime.TimeOfDay);
        }
    }
}