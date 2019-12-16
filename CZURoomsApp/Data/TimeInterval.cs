using System;
using CZURoomsApp.Services;

namespace CZURoomsApp.Data
{
    /// <summary>
    /// Třída zjednodušující práci s časovými intervaly
    /// </summary>
    public class TimeInterval
    {
        public TimeInterval(DateTime from, DateTime to)
        {
            From = from;
            To = to;
        }

        public DateTime From { get; set; }
        public DateTime To { get; set; }

        /// <summary>
        /// Komtroluje, zda se daný interval překrývá se zadaným intervalem (NE-včetně!)
        /// </summary>
        /// <param name="b">
        /// Interval, se kterým bude aktuální interval porovnán
        /// </param>
        /// <returns>
        /// True, pokud se překrývají
        /// False, pokud se nepřekrývají
        /// </returns>
        public bool OverlapsWith(TimeInterval b)
        {
            return From < b.To && To > b.From || b.To < From && b.From > To;
        }

        /// <summary>
        /// Kontroluje, zda se daný interval překrývá se zadaným intervalem JEN za pomoci porovnání času
        /// </summary>
        /// <param name="b">
        /// Interval, se kterým bude aktuální interval porovnán
        /// </param>
        /// <returns>
        /// NEZÁVISLÉ NA DATU
        /// True, pokud se překrývají
        /// False, pokud se nepřekrývají
        /// </returns>
        public bool TimeOverlapsWith(TimeInterval b)
        {
            var simplifiedA = new TimeInterval(TimeHelpers.ExtractTimeOnly(From), TimeHelpers.ExtractTimeOnly(To));
            var simplifiedB = new TimeInterval(TimeHelpers.ExtractTimeOnly(b.From), TimeHelpers.ExtractTimeOnly(b.To));

            return simplifiedA.OverlapsWith(simplifiedB);
        }

        /// <summary>
        /// Neupravuje aktuální interval.
        /// Vrátí nový interval začínající na menším datu "From" a končícím na vyšším datu "To"
        /// </summary>
        /// <param name="b">
        /// Interval, se kterým bude aktuální interval spojen
        /// </param>
        /// <returns>
        /// Nový interval vytvořený spojením aktuálního intervalu se zadaným
        /// </returns>
        public TimeInterval Concat(TimeInterval b)
        {
            var smallerFrom = From < b.From ? From : b.From;
            var largerTo = To > b.To ? To : b.To;

            return new TimeInterval(smallerFrom, largerTo);
        }
        
        public override string ToString()
        {
            return $"{From.ToString()} - {To.ToString()}";
        }
    }
}