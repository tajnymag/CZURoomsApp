using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CZURoomsApp.Data;

namespace CZURoomsApp.Services
{
    /// <summary>
    ///     Controller hlavní sekce programu.
    ///     Obsahuje metody obstarávající logické části nesouvisející přímo s grafickou vrstvou
    /// </summary>
    public static class MainController
    {
        /// <summary>
        /// Načtení seznamu akcí pro zobrazení v tabulce
        /// </summary>
        /// <param name="when">
        /// Datum dne, pro které hledáme volné místnosti
        /// </param>
        /// <returns>
        /// Task vracející strukturu s předpřipravenými daty pro tabulku v panelu hlavní sekce
        /// </returns>
        public static async Task<IEnumerable<FreeIntervalItem>> GetEvents(DateTime when)
        {
            var items = new List<FreeIntervalItem>();

            // vytvoření intervalu v daný den
            var whenInterval = TimeHelpers.GetDayInterval(when);

            // pokud nejsou načteny všechny místnosti z cache, načteme je ze serveru znovu a uložíme na disk
            if (!Store.ClassRoomRepository.IsLoaded())
            {
                var classRooms = await CzuClient.FetchAvailableClassRooms();
                
                // cache může být poškozena, tak ji raději resetujeme
                Store.ClassRoomRepository.Clear();
                
                foreach (var classRoom in classRooms)
                {
                    Store.ClassRoomRepository.Add(classRoom);
                }

                Store.ClassRoomRepository.SaveToDisk();
            }

            // načtení rozvrhových akcí z UIS
            var timetableEvents = await CzuClient.FetchTimetableEvents(when);

            // před přidáváním akcí vymažeme ty staré
            Store.ClassRoomRepository.ClearEventsOnly();
            
            // přiřadíme postupně rozvrhové akce ke správným místnostem
            foreach (var timetableEvent in timetableEvents)
            {
                Store.ClassRoomRepository.AddEvent(timetableEvent);
            }
            
            // vyfiltrování akcí a sestavení návratové hodnoty
            foreach (var classRoom in Store.ClassRoomRepository.ReadAll())
            {
                // přeskočíme místnost s názvem "všechny místnosti"
                if (classRoom.Id == 0)
                {
                    continue;
                }

                // extrahování volných intervalů pro aktuální místnost
                var freeIntervals = classRoom.GetFreeIntervals(whenInterval.From, whenInterval.To);
                
                foreach (var freeInterval in freeIntervals)
                {
                    // délka intervalu v minutách
                    var lengthInMinutes = (freeInterval.To - freeInterval.From).TotalMinutes;

                    // pokud si uživatel nepřeje zahrnout i přestávky a aktuální interval je přestavka, přeskočíme ho
                    if (!Store.Settings.IncludeBreaks && lengthInMinutes <= 15)
                    {
                        continue;
                    }
                    
                    // přidání struktury nalezeného volného intervalu místnosti do seznamu určenému pro návrat
                    var freeIntervalEvent = new FreeIntervalItem
                    {
                        Room = classRoom,
                        From = freeInterval.From,
                        To = freeInterval.To
                    };
                    items.Add(freeIntervalEvent);
                }
            }

            return items;
        }

        /// <summary>
        /// Struktura sloužící ke komunikaci nalezených dat s hlavní sekcí
        /// </summary>
        public class FreeIntervalItem
        {
            public DateTime From;
            public ClassRoom Room;
            public DateTime To;
        }
    }
}