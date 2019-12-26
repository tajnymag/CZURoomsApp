using System;
using System.IO;
using CZURoomsApp.Exceptions;

namespace CZURoomsApp.Data
{
    /// <summary>
    /// Třída spravující drobná nastavení uživatele
    /// </summary>
    public class Settings
    {
        // název souboru, do/ze kterého je nastavení načteno/uloženo
        public const string CacheFileName = "settings.ini";

        // hlavní atributy obsahující hodnoty s nastavením
        private DateTime _dayBeginTime;
        private DateTime _dayEndTime;
        private bool _includeBreaks;

        /// <summary>
        /// Od kolika hodin hledat volné místnosti
        /// </summary>
        public DateTime DayBeginTime
        {
            get => _dayBeginTime;
            set => _dayBeginTime = value;
        }

        /// <summary>
        /// Do kolika hodin hledat volné místnosti
        /// </summary>
        public DateTime DayEndTime
        {
            get => _dayEndTime;
            set => _dayEndTime = value;
        }

        /// <summary>
        /// Zda považovat místnost volnou pouze 15 minut validní
        /// </summary>
        public bool IncludeBreaks
        {
            get => _includeBreaks;
            set => _includeBreaks = value;
        }

        public Settings()
        {
            var now = DateTime.Now;

            // Defaultní hodnoty
            _dayBeginTime = new DateTime(now.Year, now.Month, now.Day, 7, 0, 0);
            _dayEndTime = new DateTime(now.Year, now.Month, now.Day, 19, 0, 0);
            _includeBreaks = false;
        }

        /// <summary>
        /// Načte nastavení z defaultního souboru a přepíše jimi aktuální hodnoty.
        /// Očekává formá INI!
        /// </summary>
        /// <exception cref="ParseErrorException">
        /// Vrací výjimku ParseErrorException, když formát souboru neobsahuje správně formátovaná data či pokud obsahuje nečekané řádky
        /// </exception>
        public void LoadFromDisk()
        {
            string line;

            // pokud soubor s cachí neexistuje, nic neděláme
            if (!File.Exists(CacheFileName))
            {
                return;
            }

            // otevření souboru
            using (StreamReader file = new StreamReader(CacheFileName))
            {
                // řádek po řádku
                while ((line = file.ReadLine()) != null)
                {
                    // rozdělení řádku dle oddělovače
                    var splitLine = line.Split('=');

                    // pokud je na řádku více než jeden oddělovač, vyhodíme výjimku
                    if (splitLine.Length != 2)
                    {
                        throw new ParseErrorException();
                    }

                    string key = splitLine[0];

                    // naparsování hodnot z aktuálního řádku
                    if (key == "DAY_BEGIN_TIME")
                    {
                        if (!DateTime.TryParse(splitLine[1], out _dayBeginTime))
                        {
                            throw new ParseErrorException();
                        }
                    }
                    else if (key == "DAY_END_TIME")
                    {
                        if (!DateTime.TryParse(splitLine[1], out _dayEndTime))
                        {
                            throw new ParseErrorException();
                        }
                    }
                    else if (key == "INCLUDE_BREAKS")
                    {
                        if (!bool.TryParse(splitLine[1], out _includeBreaks))
                        {
                            throw new ParseErrorException();
                        }
                    }
                    else
                    {
                        throw new ParseErrorException();
                    }
                }
            }
        }

        /// <summary>
        /// Uloží aktuální nastavení do defaultního souboru ve formátu INI
        /// </summary>
        public void SaveToDisk()
        {
            using (StreamWriter file = new StreamWriter(CacheFileName))
            {
                string beginTimeAsISO = _dayBeginTime.ToString("s", System.Globalization.CultureInfo.InvariantCulture);
                string endTimeAsISO = _dayEndTime.ToString("s", System.Globalization.CultureInfo.InvariantCulture);

                file.WriteLine($"DAY_BEGIN_TIME={beginTimeAsISO}");
                file.WriteLine($"DAY_END_TIME={endTimeAsISO}");
                file.WriteLine($"INCLUDE_BREAKS={IncludeBreaks.ToString()}");
            }
        }
    }
}