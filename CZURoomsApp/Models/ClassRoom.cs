using System;
using System.Collections.Generic;
using System.Linq;
using CZURoomsApp.Services;

namespace CZURoomsApp.Models
{
    public class ClassRoom
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public List<TimetableEvent> Events { get; set; }

        public ClassRoom(int id, string name)
        {
            Name = name;
            Id = id;
            Events = new List<TimetableEvent>();
        }

        public IEnumerable<TimeInterval> GetFreeIntervals(DateTime from, DateTime to)
        {
            var freeQuarters =
                TimeHelpers.FindFreeQuarters(Events.Select(interval => interval.Interval).ToList(), from, to);
            var freeIntervals = new List<TimeInterval>();
            
            TimeInterval lastFreeInterval = null;
            foreach (var currentFreeQuarter in freeQuarters)
            {
                if (lastFreeInterval == null)
                {
                    lastFreeInterval = currentFreeQuarter;
                    continue;
                }

                if (currentFreeQuarter.From == lastFreeInterval.To)
                {
                    lastFreeInterval = lastFreeInterval.Concat(currentFreeQuarter);
                }
                else
                {
                    freeIntervals.Add(lastFreeInterval);
                    lastFreeInterval = currentFreeQuarter;
                }
            }

            freeIntervals.Add(lastFreeInterval);

            return freeIntervals;
        }
    }
}