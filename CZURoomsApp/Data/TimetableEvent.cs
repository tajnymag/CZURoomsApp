using CZURoomsApp.Enums;

namespace CZURoomsApp.Data
{
    /// <summary>
    /// Třída sloužící k reprezentaci rozvrhové akce
    /// </summary>
    public class TimetableEvent
    {
        public TimetableEventType EventType;
        public TimeInterval Interval;
        public ClassRoom Room;
        public string Subject;
    }
}