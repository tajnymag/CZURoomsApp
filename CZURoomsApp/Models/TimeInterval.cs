using System;
using CZURoomsApp.Services;

namespace CZURoomsApp.Models
{
    public class TimeInterval
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }

        public TimeInterval(DateTime from, DateTime to)
        {
            From = from;
            To = to;
        }
        
        public bool OverlapsWith(TimeInterval b)
        {
            return (this.From < b.To && this.To > b.From) || (b.To < this.From && b.From > this.To);
        }

        public bool TimeOverlapsWith(TimeInterval b)
        {
            var simplifiedA = new TimeInterval(TimeHelpers.ExtractTimeOnly(From), TimeHelpers.ExtractTimeOnly(To));
            var simplifiedB = new TimeInterval(TimeHelpers.ExtractTimeOnly(b.From), TimeHelpers.ExtractTimeOnly(b.To));

            return simplifiedA.OverlapsWith(simplifiedB);
        }

        public TimeInterval AddMinutes(int minutes)
        {
            From = From.AddMinutes(15);
            To = To.AddMinutes(15);

            return this;
        }

        public TimeInterval Concat(TimeInterval b)
        {
            var smallerFrom = (this.From < b.From) ? this.From : b.From;
            var largerTo = (this.To > b.To) ? this.To : b.To;
            
            return new TimeInterval(smallerFrom, largerTo);
        }
        
        public override string ToString()
        {
            return $"{From.ToString()} - {To.ToString()}";
        }
    }
}