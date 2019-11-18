using System;
using System.Globalization;
using System.Text;
using CZURoomsApp.Enums;
using CZURoomsApp.Models;
using HtmlAgilityPack;

namespace CZURoomsApp.Services
{
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
            string room = columnEls[5].InnerText;
            
            timetableEvent.Interval = new TimeInterval(ParseDateTime(date, from), ParseDateTime(date, to));
            timetableEvent.Room = room;
            timetableEvent.Subject = subject;

            if (eventType == "Přednáška" || eventType == "Lecture")
            {
                timetableEvent.EventType = TimetableEventType.Lecture;
            }
            
            return timetableEvent;
        }

        private DateTime ParseDateTime(string date, string time)
        {
            string dateWithoutDay = date.Split(' ')[1];
            string dateTimeToParse = $"{dateWithoutDay} {time}";
            
            return DateTime.ParseExact(dateTimeToParse, "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture);
        }
    }
}