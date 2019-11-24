using System;
using CZURoomsApp.Enums;
using CZURoomsApp.Models;

namespace CZURoomsApp.Models
{
    public class TimetableEvent
    {
        public TimeInterval Interval;
        public string Subject;
        public ClassRoom Room;
        public TimetableEventType EventType;
    }
}