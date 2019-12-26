using System.Collections.Generic;
using System.IO;
using CZURoomsApp.Exceptions;

namespace CZURoomsApp.Data
{
    /// <summary>
    /// Třída spravující repozitář s načtenými místnostmi a jejich rozvrhových akcí
    /// </summary>
    public class ClassRoomRepository
    {
        // konstanta sloužící k ověření správného počtu načtených místností (aktuální počet na webu)
        public const int ExpectedClassRoomsCount = 88;

        // název souboru, do/ze kterého je repozitář načítán/uložen
        public const string CacheFileName = "rooms.csv";

        /// <summary>
        /// Mapa držící hlavní data této třídy.
        /// Klíč: název místnosti
        /// Hodnota: objekt místnosti
        /// </summary>
        private readonly Dictionary<string, ClassRoom> _classRooms;

        public ClassRoomRepository()
        {
            _classRooms = new Dictionary<string, ClassRoom>();
        }

        /// <summary>
        /// Vyhledání místnosti dle jejího jména
        /// </summary>
        /// <param name="name">
        /// Povinný parametr ozančující oficiální název místnosti
        /// </param>
        /// <returns>
        /// Objekt místnosti nebo vyhodí výjimku
        /// </returns>
        public ClassRoom GeByName(string name)
        {
            return _classRooms[name];
        }

        /// <summary>
        /// Read-only přístup k seznamu místností
        /// </summary>
        /// <returns>
        /// Iterátor hlavní mapy ukazující jednotlivé objekty místností
        /// </returns>
        public IEnumerable<ClassRoom> ReadAll()
        {
            foreach (var entry in _classRooms)
            {
                yield return entry.Value;
            }
        }

        /// <summary>
        /// Přidání rozvrhové akce ke správné místnosti
        /// </summary>
        /// <param name="timetableEvent">
        /// Povinný parametr označující objekt rozvrhové akce
        /// </param>
        public void AddEvent(TimetableEvent timetableEvent)
        {
            _classRooms[timetableEvent.Room.Name].Events.Add(timetableEvent);
        }

        /// <summary>
        /// Bezpečně přidá místnost do hlavní mapy
        /// </summary>
        /// <param name="classRoom">
        /// Označuje objekt místnosti jež bude vložena do hlavní mapy
        /// </param>
        public void Add(ClassRoom classRoom)
        {
            if (!_classRooms.ContainsKey(classRoom.Name)) _classRooms.Add(classRoom.Name, classRoom);
        }

        /// <summary>
        /// Zkontroluje, že jsou načteny všechny, na webu dostupné, místnosti
        /// </summary>
        /// <returns>
        /// True, když je počet načtených místností stejný jako počet místnosti v UIS
        /// False, když není počet načtených místností stejný jako počet místnosti v UIS
        /// </returns>
        public bool IsLoaded()
        {
            return _classRooms.Count == ExpectedClassRoomsCount;
        }

        /// <summary>
        /// Vymaže veškeré záznamy z hlavní mapy
        /// </summary>
        public void Clear()
        {
            _classRooms.Clear();
        }

        /// <summary>
        /// Vymaže seznamy rozvrhových akcí u všech místností
        /// </summary>
        public void ClearEventsOnly()
        {
            foreach (var entry in _classRooms)
            {
                entry.Value.Events.Clear();
            }
        }

        /// <summary>
        /// Načte mapu místností z defaultního souboru a přepíše jí aktuální hodnoty
        /// Očekává formát CSV!
        /// </summary>
        /// <exception cref="ParseErrorException">
        /// Vrací výjimku ParseErrorException, když formát souboru neobsahuje správně formátovaná data či pokud se načte nečekané množství místností
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
                Clear();

                // řádek po řádku
                while ((line = file.ReadLine()) != null)
                {
                    // rozdělení řádku dle oddělovače
                    var splitLine = line.Split(',');

                    // pokud je na řádku více než jeden oddělovač, vyhodíme výjimku
                    if (splitLine.Length != 2)
                    {
                        throw new ParseErrorException();
                    }

                    // naparsování hodnot z aktuálního řádku
                    string name = splitLine[1];
                    int id;
                    if (!int.TryParse(splitLine[0], out id))
                    {
                        throw new ParseErrorException();
                    }

                    Add(new ClassRoom(id, name));
                }

                // kontrola počtu načtených místností
                if (!IsLoaded())
                {
                    throw new ParseErrorException("Neočekávaná délka seznamu");
                }
            }
        }

        /// <summary>
        /// Uloží seznam místností do defaultního souboru ve formátu CSV
        /// </summary>
        public void SaveToDisk()
        {
            using (StreamWriter file = new StreamWriter(CacheFileName))
            {
                foreach (var classRoom in _classRooms)
                {
                    var name = classRoom.Value.Name;
                    var id = classRoom.Value.Id;

                    file.WriteLine($"{id},{name}");
                }
            }
        }
    }
}