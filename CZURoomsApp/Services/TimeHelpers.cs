using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using CZURoomsApp.Models;

namespace CZURoomsApp.Services
{
    public static class TimeHelpers
    {
        public static DateTime ExtractTimeOnly(DateTime dateTime)
        {
            return default(DateTime).Add(dateTime.TimeOfDay);
        }

        public static TimeInterval GetDayInterval(DayOfWeek dayOfWeek, DateTime from, DateTime to)
        {
            var date = from;

            while (date.DayOfWeek != dayOfWeek && date < to)
            {
                date = date.AddDays(1);
            }

            var beginningOfTheDay = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
            
            return new TimeInterval(beginningOfTheDay, beginningOfTheDay.AddHours(24));
        }
        
        public static IEnumerable<TimeInterval> Quarters(DateTime from, DateTime to)
        {
            int numberOfQuarters = (int)to.Subtract(from).TotalMinutes / 15;
            
            for (int i = 0; i < numberOfQuarters; ++i)
            {
                yield return new TimeInterval(from.AddMinutes(i * 15), from.AddMinutes((i + 1) * 15));
            }
        }

        public static IEnumerable<TimeInterval> FindFreeQuarters(List<TimeInterval> intervals, DateTime from, DateTime to)
        {
            foreach (var quarter in Quarters(from, to))
            {
                bool isFree = true;
                
                foreach (var interval in intervals)
                {
                    if (quarter.TimeOverlapsWith(interval))
                    {
                        isFree = false;
                    }
                }

                if (isFree)
                {
                    yield return quarter;
                }
            }
        }
    }
}