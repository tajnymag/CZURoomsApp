using System;
using CZURoomsApp.Enums;

namespace CZURoomsApp.Models
{
    public class TimetableEvent
    {
        public TimeInterval Interval;
        public string Subject;
        public string Room;
        public TimetableEventType EventType;
    }
}