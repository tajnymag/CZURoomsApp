using System;
using System.Collections.Generic;
using CZURoomsApp.Data;

namespace CZURoomsApp.Services
{
    /// <summary>
    /// Sada metod pro jednodušší práci s časovými problematikami
    /// </summary>
    public static class TimeHelpers
    {
        /// <summary>
        /// Normalizuje zadaný objekt data tak, aby neoznačoval žádné reálné informace o dnu/měsíci/dnu
        /// </summary>
        /// <param name="dateTime">
        /// Objekt data, který chceme normalizovat
        /// </param>
        /// <returns>
        /// Vyčištěný objekt data o informace o kalendářních informacích
        /// </returns>
        public static DateTime ExtractTimeOnly(DateTime dateTime)
        {
            return default(DateTime).Add(dateTime.TimeOfDay);
        }

        /// <summary>
        /// Vrátí interval školního dne v zadaný den
        /// </summary>
        /// <param name="date">
        /// Den, ve kterém se daný interval má nacházet
        /// </param>
        /// <returns>
        /// Interval školního dne
        /// </returns>
        public static TimeInterval GetDayInterval(DateTime date)
        {
            // sestavení počátku a konce dne v závisloti na přijatém datu a časech ve storu
            var beginningOfTheDay = new DateTime(date.Year, date.Month, date.Day, Store.Settings.DayBeginTime.Hour,
                Store.Settings.DayBeginTime.Minute, 0);
            var endOfTheDay = new DateTime(date.Year, date.Month, date.Day, Store.Settings.DayEndTime.Hour,
                Store.Settings.DayEndTime.Minute, 0);

            return new TimeInterval(beginningOfTheDay, endOfTheDay);
        }

        /// <summary>
        /// Sestaví seznam všech čtvrt-hodin mezi zadanými časy
        /// </summary>
        /// <param name="from">
        /// Začátek první čtvrt-hodiny
        /// </param>
        /// <param name="to">
        /// Konec poslední čtvrt-hodiny
        /// </param>
        /// <returns>
        /// Seznam všech čtvrt-hodin mezi zadanými časy
        /// </returns>
        public static IEnumerable<TimeInterval> Quarters(DateTime from, DateTime to)
        {
            // počet čtvrt-hodin v daném intervalu
            var numberOfQuarters = (int) to.Subtract(from).TotalMinutes / 15;
            // inicializace seznamu
            var quarters = new List<TimeInterval>(numberOfQuarters);

            // naplnění seznamu jednotlivými instancemi čtvrt-hodin jako časových intervalů
            for (var i = 0; i < numberOfQuarters; ++i)
            {
                quarters.Add(new TimeInterval(@from.AddMinutes(i * 15), @from.AddMinutes((i + 1) * 15)));
            }

            return quarters;
        }

        /// <summary>
        /// Nalezení intervalů mezi zadanými časy, jež se nepřekrývají s žádným z intervalu ze zadaného seznamu
        /// </summary>
        /// <param name="intervals">
        /// Seznam časových intervalů, ve kterých se něco děje
        /// </param>
        /// <param name="from">
        /// Počátek intervalu, ve kterém se hledají konflikty se zadaným seznamem
        /// </param>
        /// <param name="to">
        /// Konec intervalu, ve kterém se hledají konflikty se zadaným seznamem
        /// </param>
        /// <returns>
        /// Seznam krátkých intervalů, které se nepřekrývají se zadaným seznamem
        /// </returns>
        public static IEnumerable<TimeInterval> FindFreeQuarters(List<TimeInterval> intervals, DateTime from,
            DateTime to)
        {
            var freeQuarters = new List<TimeInterval>();

            // každou čtvrt-hodinu zvlášť
            foreach (var quarter in Quarters(from, to))
            {
                // předpokládáme nejdříve, že se s žádným intervalem nepřekrývá
                var isFree = true;

                // zkontrolujeme, že se s žádným intervalem nepřekrývá
                // pokud ano, označíme ji jako zaplňenou a můžeme přejít na další čtvrt-hodinu
                foreach (var interval in intervals)
                {
                    if (quarter.TimeOverlapsWith(interval))
                    {
                        isFree = false;
                        break;
                    }
                }

                // pokud se s žádným intervalem nepřekrývala, přidáme ji do seznamu volných
                if (isFree)
                {
                    freeQuarters.Add(quarter);
                }
            }

            return freeQuarters;
        }
    }
}