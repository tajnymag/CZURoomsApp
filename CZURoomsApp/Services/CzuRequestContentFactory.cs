using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using CZURoomsApp.Data;

namespace CZURoomsApp.Services
{
    /// <summary>
    /// Sada metod sloužící k přípravě požadavků pro UIS
    /// </summary>
    public static class CzuRequestContentFactory
    {
        /// <summary>
        /// Převede objekt do požadavku ve formátu FormUrl
        /// </summary>
        /// <param name="parameters">
        /// Objekt s parametry formuláře
        /// </param>
        /// <returns>
        /// Hotový požadavek pro .NET HttpClient
        /// </returns>
        private static HttpContent FormUrlEncodedContent(IEnumerable<KeyValuePair<string, string>> parameters)
        {
            var formUrl = "";

            foreach (var parameter in parameters)
            {
                formUrl += $"{parameter.Key}={parameter.Value}&";
            }

            return new StringContent(formUrl, Encoding.UTF8, "application/x-www-form-urlencoded");
        }

        /// <summary>
        /// Vytvoří požadavek na přihlášení do systému
        /// </summary>
        /// <param name="username">
        /// Uživatelské jméno
        /// </param>
        /// <param name="password">
        /// Uživatelské heslo
        /// </param>
        /// <param name="loginHidden"></param>
        /// <param name="destination"></param>
        /// <param name="authIdHidden"></param>
        /// <param name="cookieLife">
        /// Doba platnosti přihlašovací cookie v sekundách.
        /// Aktuálně nejvyšší hodnota vybratelná v rozhraní jsou 4 dny => 4d * 24h * 60m * 60s = 345600s
        /// </param>
        /// <param name="credentialK"></param>
        /// <returns>
        /// Hotový požadavek pro .NET HttpClient
        /// </returns>
        public static HttpContent GetAuth(string username, string password,
            bool loginHidden = true,
            string destination = "/auth", bool authIdHidden = false, int cookieLife = 345600, string credentialK = "")
        {
            // přiřazení přijatých parametrů ke klíčovým heslům pro UIS
            var parameters = new[]
            {
                new KeyValuePair<string, string>("login_hidden", loginHidden ? "1" : "0"),
                new KeyValuePair<string, string>("destination", destination),
                new KeyValuePair<string, string>("auth_id_hidden", authIdHidden ? "1" : " 0"),
                new KeyValuePair<string, string>("credential_0", username),
                new KeyValuePair<string, string>("credential_1", password),
                new KeyValuePair<string, string>("credential_2", cookieLife.ToString()),
                new KeyValuePair<string, string>("credential_k", credentialK)
            };

            return FormUrlEncodedContent(parameters);
        }

        /// <summary>
        /// Vytvoří poždadavek na stranu rozvrhu místnosti dle zadaných podmínek
        /// </summary>
        /// <param name="room">
        /// UIS Id místnosti
        /// </param>
        /// <param name="dayOfWeek">
        /// Den v týdnu
        /// </param>
        /// <param name="from">
        /// Počátek časového rozmezí vyhledávání
        /// </param>
        /// <param name="to">
        /// Konec časového rozmezí vyhledávání
        /// </param>
        /// <param name="z"></param>
        /// <param name="k"></param>
        /// <param name="timetable"></param>
        /// <param name="baseNotes"></param>
        /// <param name="changes">
        /// Zda zahrnout i změny v rozvrhu
        /// </param>
        /// <param name="pairing"></param>
        /// <param name="differentTeacher">
        /// Zda zahrnout i změny způsobené změnou vyučujícího
        /// </param>
        /// <param name="differentLocation">
        /// Zda zahrnout i změny způsobené přesunem místnosti
        /// </param>
        /// <param name="otherChanges">
        /// Zda zahrnout ostatní změny
        /// </param>
        /// <param name="displayMode">
        /// Způsob zobrazení rozvrhu
        /// Možnosti: "konani", "souhrn" a "vyjimky"
        /// </param>
        /// <param name="format">
        /// Formát rozvrhu
        /// Možnosti: "list", "html" a "pdf"
        /// </param>
        /// <param name="reservations">
        /// Zda zahrnout i rezervace
        /// </param>
        /// <param name="notes">
        /// Zda zahrnout i poznámky
        /// </param>
        /// <param name="show"></param>
        /// <param name="show2"></param>
        /// <returns>
        /// Hotový požadavek pro .NET HttpClient
        /// </returns>
        public static HttpContent GetRoom(ClassRoom room, DayOfWeek dayOfWeek,
            DateTime from, DateTime to, int z = 1,
            int k = 1, int timetable = 1000, bool baseNotes = true, bool changes = true, bool pairing = true,
            bool differentTeacher = true,
            bool differentLocation = true, bool otherChanges = true, string displayMode = "konani",
            string format = "list",
            bool reservations = true, bool notes = true, bool show = true, string show2 = "Zobrazit")
        {
            // převedení systémové reprezentace dneVTýdnu na UIS formát
            var correctedDayOfWeek = (int) dayOfWeek;
            if (correctedDayOfWeek == 6)
            {
                correctedDayOfWeek = 7;
            }

            // naformátování stringu data do UIS formátu
            var fromDay = from.Day.ToString();
            var fromMonth = from.Month.ToString();
            var toDay = to.Day.ToString();
            var toMonth = to.Month.ToString();

            var formattedFrom = $"{fromDay}.+{fromMonth}.+{from.Year}";
            var formattedTo = $"{toDay}.+{toMonth}.+{to.Year}";

            // přiřazení přijatých parametrů ke klíčovým heslům pro UIS
            var parameters = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("den", correctedDayOfWeek.ToString()),
                new KeyValuePair<string, string>("z", z.ToString()),
                new KeyValuePair<string, string>("k", k.ToString()),
                new KeyValuePair<string, string>("rozvrh", timetable.ToString()),
                new KeyValuePair<string, string>("poznamky", notes ? "1" : "0"),
                new KeyValuePair<string, string>("poznamky_base", baseNotes ? "1" : "0"),
                new KeyValuePair<string, string>("poznamky_zmeny", changes ? "1" : "0"),
                new KeyValuePair<string, string>("poznamky_parovani", pairing ? "1" : "0"),
                new KeyValuePair<string, string>("poznamky_dalsi_ucit", differentTeacher ? "1" : "0"),
                new KeyValuePair<string, string>("poznamky_jiny_areal", differentLocation ? "1" : "0"),
                new KeyValuePair<string, string>("poznamky_dl_omez", otherChanges ? "1" : "0"),
                new KeyValuePair<string, string>("typ_vypisu", displayMode),
                new KeyValuePair<string, string>("konani_od", formattedFrom),
                new KeyValuePair<string, string>("konani_do", formattedTo),
                new KeyValuePair<string, string>("format", format),
                new KeyValuePair<string, string>("rezervace", reservations ? "1" : "0"),
                new KeyValuePair<string, string>("zobraz", show ? "1" : "0"),
                new KeyValuePair<string, string>("zobraz2", show2)
            };

            // pokud není vyžádána místnost 0 => kód pro všechny místnosti => přidáme ještě specifikátor id místnosti
            if (room.Id != 0)
            {
                parameters.Add(new KeyValuePair<string, string>("mistnost", room.Id.ToString()));
            }

            return FormUrlEncodedContent(parameters);
        }
    }
}