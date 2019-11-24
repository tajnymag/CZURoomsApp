using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using CZURoomsApp.Enums;
using CZURoomsApp.Models;
using HtmlAgilityPack;

namespace CZURoomsApp.Services
{
    public class NoEventsFoundException : Exception
    {
        public NoEventsFoundException() {}
        public NoEventsFoundException(string message): base(message) {}
        public NoEventsFoundException(string message, Exception inner): base(message, inner) {}
    }
    
    public class CZUParser
    {
        private HtmlDocument _doc;
        
        public CZUParser(string htmlString)
        {
            _doc = new HtmlDocument();
            _doc.LoadHtml(htmlString);
        }

        private HtmlNode GetMainTable()
        {
            var mainTable = _doc.GetElementbyId("tmtab_1");

            if (mainTable == null)
            {
                throw new NoEventsFoundException();
            }
            
            return mainTable;
        }

        public HtmlNodeCollection GetAllRows()
        {
            var mainTable = GetMainTable();
            var rows = mainTable.SelectNodes("tbody/tr");

            return rows;
        }

        public TimetableEvent GetTimetableEvent(HtmlNode rowElement)
        {
            var timetableEvent = new TimetableEvent();
            var columnEls = rowElement.SelectNodes("td[contains(@class, 'odsazena')]");

            string date = columnEls[0].InnerText;
            string from = columnEls[1].InnerText;
            string to = columnEls[2].InnerText;
            string subject = columnEls[3].InnerText;
            string eventType = columnEls[4].InnerText;
            string roomName = columnEls[5].InnerText;
            
            timetableEvent.Interval = new TimeInterval(ParseDateTime(date, from), ParseDateTime(date, to));
            timetableEvent.Room = Shared.GetClassRoomByName(roomName);
            timetableEvent.Subject = subject;

            if (eventType == "Přednáška" || eventType == "Lecture")
            {
                timetableEvent.EventType = TimetableEventType.Lecture;
            }
            
            return timetableEvent;
        }

        public List<TimetableEvent> GetTimetableEvents(HtmlNodeCollection rowElements)
        {
            var events = new List<TimetableEvent>();
            
            foreach (var rowEl in rowElements)
            {
                events.Add(GetTimetableEvent(rowEl));
            }

            return events;
        }

        public List<ClassRoom> GetClassRooms()
        {
            var classRooms = new List<ClassRoom>();
            
            var roomSelectEl = _doc.DocumentNode.SelectNodes("//select[contains(@name, 'mistnost')]").First();
            var roomEls = roomSelectEl.ChildNodes;

            foreach (var roomEl in roomEls)
            {
                var classRoom = new ClassRoom(Int32.Parse(roomEl.Attributes["value"].Value), roomEl.InnerText);
                classRooms.Add(classRoom);
            }

            return classRooms;
        }

        private DateTime ParseDateTime(string date, string time)
        {
            string dateWithoutDay = date.Split(' ')[1];
            string dateTimeToParse = $"{dateWithoutDay} {time}";
            
            return DateTime.ParseExact(dateTimeToParse, "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture);
        }
    }
}