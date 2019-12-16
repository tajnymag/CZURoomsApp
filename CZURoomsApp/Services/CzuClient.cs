using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CZURoomsApp.Data;

namespace CZURoomsApp.Services
{
    /// <summary>
    /// Sada metod pro získání čistých dat ze serveru UIS
    /// </summary>
    public static class CzuClient
    {
        /// <summary>
        /// Získá seznam místností ze serveru UIS
        /// </summary>
        /// <returns>
        /// Seznam s objekty místností
        /// </returns>
        public static async Task<IEnumerable<ClassRoom>> FetchAvailableClassRooms()
        {
            await Store.Uis.Login();
            
            // stažení html potřebné strany
            var selectionPageHtml = await Store.Uis.GetEnumsPage();
            // vytvoření parseru na dané html straně
            var selectionParser = new CzuParser(selectionPageHtml);
            // extrahování seznamu místností z dané strany
            var classRooms = selectionParser.GetClassRooms();

            return classRooms;
        }

        /// <summary>
        /// Získá seznam rozvrhových akcí pro zadanou místnost v zadaném časovém rozpětí
        /// </summary>
        /// <param name="room">
        /// Validní objekt místnosti
        /// </param>
        /// <param name="from">
        /// Počátek data rozsahu rovzhu
        /// </param>
        /// <param name="to">
        /// Koncová data rozsahu rozvrhu
        /// </param>
        /// <param name="dayOfWeek">
        /// Den v týdnu
        /// </param>
        /// <returns>
        /// Seznam rozvrhových akcí
        /// </returns>
        public static async Task<IEnumerable<TimetableEvent>> FetchTimetableEvents(ClassRoom room, DateTime from,
            DateTime to, DayOfWeek dayOfWeek)
        {
            await Store.Uis.Login();
            
            // stažení html potřebné strany
            var timetablePageHtml = await Store.Uis.GetRoomPage(room, from,
                to, dayOfWeek);
            // vytvoření parseru na dané html straně
            var parser = new CzuParser(timetablePageHtml);
            // extrahování seznamu rozvrhových akcí z dané strany
            var timetableEvents = parser.GetTimetableEvents(parser.GetAllRows());

            return timetableEvents;
        }

        /// <summary>
        /// Získá seznam rozvrhových akcí VŠECH MÍSTNOSTÍ v zadaný den
        /// </summary>
        /// <param name="when">
        /// Datum pro které hledáme akce
        /// </param>
        /// <returns>
        /// Seznam rozvrhových akcí pro všechny místnosti
        /// </returns>
        public static async Task<IEnumerable<TimetableEvent>> FetchTimetableEvents(DateTime when)
        {
            return await FetchTimetableEvents(new ClassRoom(0, "-- všechny místnosti --"), when, when, when.DayOfWeek);
        }
    }
}