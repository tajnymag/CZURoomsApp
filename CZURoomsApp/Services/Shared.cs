using System;
using System.Collections.Generic;
using CZURoomsApp.Models;

namespace CZURoomsApp.Services
{
    public static class Shared
    {
        private static Dictionary<string, ClassRoom> _classRooms;
        public static CZUApi Uis { get; set; }

        public static ClassRoom GetClassRoomByName(string name)
        {
            return _classRooms[name];
        }

        public static void AddEventToClassRoom(string name, TimetableEvent timetableEvent)
        {
            _classRooms[name].Events.Add(timetableEvent);
        }

        public static void AddClassRoom(string name, ClassRoom classRoom)
        {
            if (_classRooms.ContainsKey(name))
            {
                _classRooms.Add(name, classRoom);
            }
        }
    }
}