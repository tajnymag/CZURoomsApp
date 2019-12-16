using CZURoomsApp.Services;

namespace CZURoomsApp.Data
{
    /// <summary>
    /// Statická třída spravující sdílený store pro celou aplikaci
    /// </summary>
    public static class Store
    {
        public static ClassRoomRepository ClassRoomRepository { get; }
        public static CzuApi Uis { get; }
        public static Settings Settings { get; }

        static Store()
        {
            ClassRoomRepository = new ClassRoomRepository();
            Uis = new CzuApi();
            Settings = new Settings();
        }

        /// <summary>
        /// Uložení seznamu místností a nastavení do defaultní cesty
        /// </summary>
        public static void LoadFromDisk()
        {
            ClassRoomRepository.LoadFromDisk();
            Settings.LoadFromDisk();
        }
    }
}