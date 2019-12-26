using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CZURoomsApp.Data;
using CZURoomsApp.Enums;
using CZURoomsApp.Exceptions;
using HtmlAgilityPack;

namespace CZURoomsApp.Services
{
    /// <summary>
    /// Slouží k extrahování pro nás důležitých informací z html stringů
    /// </summary>
    public class CzuParser
    {
        private readonly HtmlDocument _doc;

        public CzuParser(string htmlString)
        {
            // načtení html do virtuálního DOM
            _doc = new HtmlDocument();
            _doc.LoadHtml(htmlString);
        }

        /// <summary>
        /// Extrahuje prvek hlavní tabulky strany
        /// </summary>
        /// <returns>
        /// Objekt prvku hlavní tabulky
        /// </returns>
        /// <exception cref="NoEventsFoundException">
        /// Pokud na straně hledaná tabulka není, vrátí NoEventsFoundException
        /// </exception>
        private HtmlNode GetMainTable()
        {
            var mainTable = _doc.GetElementbyId("tmtab_1");

            if (mainTable == null)
            {
                throw new NoEventsFoundException();
            }

            return mainTable;
        }

        /// <summary>
        /// Extrahuje seznam prvků všech řádků hlavní tabulky
        /// </summary>
        /// <returns>
        /// Seznam všech prvků řádků hlavní tabulky
        /// </returns>
        public HtmlNodeCollection GetAllRows()
        {
            var mainTable = GetMainTable();
            var rows = mainTable.SelectNodes("tbody/tr");

            return rows;
        }

        /// <summary>
        /// Extrahuje z řádku objekt rozvrhové akce
        /// </summary>
        /// <param name="rowElement">
        /// Prvek řádku
        /// </param>
        /// <returns>
        /// Objekt rozvrhové akce
        /// </returns>
        /// <exception cref="ParseErrorException">
        /// Pokud se nepodaří načíst typ akce, vyhodí ParseErrorException
        /// </exception>
        public TimetableEvent GetTimetableEvent(HtmlNode rowElement)
        {
            var timetableEvent = new TimetableEvent();
            // získáme prvky sloupců
            var columnEls = rowElement.SelectNodes("td[contains(@class, 'odsazena')]");

            // rozřazení stringových hodnot ze sloupců
            var date = columnEls[0].InnerText;
            var from = columnEls[1].InnerText;
            var to = columnEls[2].InnerText;
            var subject = columnEls[3].InnerText;
            var eventType = columnEls[4].InnerText;
            var roomName = columnEls[5].InnerText;
        
            // vyparsování textových hodnot do objektu intervalu
            timetableEvent.Interval = new TimeInterval(ParseDateTime(date, from), ParseDateTime(date, to));
            // pokusíme se načíst místnost dle názvu z tabulky v repozitáři dříve získaných místností
            timetableEvent.Room = Store.ClassRoomRepository.GeByName(roomName);
            timetableEvent.Subject = subject;

            // přirazení správného typu dle textové hodnoty
            if (eventType == "Přednáška" || eventType == "Lecture")
            {
                timetableEvent.EventType = TimetableEventType.Lecture;
            }
            else if (eventType == "Cvičení" || eventType == "Seminar")
            {
                timetableEvent.EventType = TimetableEventType.Seminar;
            }
            else if (eventType == "Rezervace" || eventType == "Reservation")
            {
                timetableEvent.EventType = TimetableEventType.Reservation;
            }
            else
            {
                throw new ParseErrorException();
            }

            return timetableEvent;
        }

        /// <summary>
        /// Extrahuje seznam rozvrhových akcí z řádků tabulky.
        /// </summary>
        /// <param name="rowElements">
        /// Seznam prvků řádků tabulky
        /// </param>
        /// <returns>
        /// Seznam rozvhových akcí
        /// </returns>
        public IEnumerable<TimetableEvent> GetTimetableEvents(HtmlNodeCollection rowElements)
        {
            var events = new List<TimetableEvent>();

            foreach (var rowEl in rowElements)
            {
                events.Add(GetTimetableEvent(rowEl));
            }

            return events;
        }

        /// <summary>
        /// Extrahuje seznam místností
        /// </summary>
        /// <returns>
        /// Seznam místností
        /// </returns>
        public IEnumerable<ClassRoom> GetClassRooms()
        {
            var classRooms = new List<ClassRoom>();

            var roomSelectEl = _doc.DocumentNode.SelectNodes("//select[contains(@name, 'mistnost')]").First();
            var roomEls = roomSelectEl.ChildNodes;

            foreach (var roomEl in roomEls)
            {
                var classRoom = new ClassRoom(int.Parse(roomEl.Attributes["value"].Value), roomEl.InnerText);
                classRooms.Add(classRoom);
            }

            return classRooms;
        }

        /// <summary>
        /// Naparsuje objekt data z textových hodnot
        /// </summary>
        /// <param name="date">
        /// Textová podoba data z tabulky
        /// </param>
        /// <param name="time">
        /// Textová podoba času z tabulky
        /// </param>
        /// <returns>
        /// Validní objekt data
        /// </returns>
        private DateTime ParseDateTime(string date, string time)
        {
            // odmažeme název dne
            var dateWithoutDay = date.Split(' ')[1];
            // připravíme string s datem do podoby, jež dokáže .NET nativně naparsovat
            var dateTimeToParse = $"{dateWithoutDay} {time}";

            return DateTime.ParseExact(dateTimeToParse, "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture);
        }
    }
}