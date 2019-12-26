using System;
using System.Collections.Generic;
using System.Linq;
using CZURoomsApp.Services;

namespace CZURoomsApp.Data
{
    /// <summary>
    ///     Třída sloužící k práci s místností jako objektem
    /// </summary>
    public class ClassRoom
    {
        public ClassRoom(int id, string name)
        {
            Name = name;
            Id = id;
            Events = new List<TimetableEvent>();
        }

        /// <summary>
        /// Oficiální název místnosti
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Id místnosti v UIS
        /// Slouží k případnému filtrování dle místnosti
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Seznam rozvrhových akcí v dané místnosti
        /// </summary>
        public List<TimetableEvent> Events { get; set; }

        /// <summary>
        /// Vrátí seznam časových intervalů, ve kterých se v místnosti něděje žádná rozvrhová akce
        /// </summary>
        /// <param name="from">
        /// Od jakého času máme hledat volné intervaly
        /// </param>
        /// <param name="to">
        /// Do jakého času máme hledat volné intervaly
        /// </param>
        /// <returns>
        /// Seznam časových intervalů, ve kterých se v místnosti něděje žádná rozvrhová akce
        /// </returns>
        public IEnumerable<TimeInterval> GetFreeIntervals(DateTime from, DateTime to)
        {
            // seznam s čtvrt-hodinami, ve které se neděje žádná rozvrhová akce
            var freeQuarters =
                TimeHelpers.FindFreeQuarters(Events.Select(interval => interval.Interval).ToList(), from, to);
            // inicializace seznamu intervalů, které později vrátíme
            var freeIntervals = new List<TimeInterval>();

            // spojování čtvrt-hodin v co nejdelší interval
            TimeInterval lastFreeInterval = null;
            foreach (var currentFreeQuarter in freeQuarters)
            {
                // první čtvrt-hodina v seznamu => první naplnění posledního intervalu pro další iterace
                if (lastFreeInterval == null)
                {
                    lastFreeInterval = currentFreeQuarter;
                    continue;
                }
                
                // pokud začátek aktuální čtvrt-hodiny je stejný, jako konec posledního tvořeného intervalu, spojíme je do jednoho delšího
                if (currentFreeQuarter.From == lastFreeInterval.To)
                {
                    lastFreeInterval = lastFreeInterval.Concat(currentFreeQuarter);
                }
                // pokud ne, přidáme poslední vytvořený interval do seznamu intervalů a resetujeme naši pomocnou proměnnou na aktuální volnou čtvrt-hodinu
                else
                {
                    freeIntervals.Add(lastFreeInterval);
                    lastFreeInterval = currentFreeQuarter;
                }
            }

            // přidání posledního intervalu do seznamu intervalů
            freeIntervals.Add(lastFreeInterval);

            return freeIntervals;
        }
    }
}