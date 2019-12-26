using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CZURoomsApp.Data;
using CZURoomsApp.Exceptions;

namespace CZURoomsApp.Services
{
    /// <summary>
    /// Třída obstarávající komunikaci se serverem UIS
    /// </summary>
    public class CzuApi
    {
        private readonly CookieContainer _cookieContainer;
        private readonly HttpClient _httpClient;
        public readonly Uri UisUri = new Uri("https://is.czu.cz");

        private string _password;
        private string _username;

        public CzuApi()
        {
            // inicializace vestavěného http klienta
            _cookieContainer = new CookieContainer();
            _httpClient = new HttpClient(new HttpClientHandler {CookieContainer = _cookieContainer});
            
            // informování serveru UIS, že chceme dostat odpověď v češtině
            _httpClient.DefaultRequestHeaders.Add("Accept-Language", "cs,en;q=0.5");
        }

        public CzuApi(string username, string password) : this()
        {
            _username = username;
            _password = password;
        }
        
        public CzuApi(Cookie cookie) : this()
        {
            _cookieContainer.Add(new Uri(cookie.Path), cookie);
        }

        public void UpdateCredentials(string username, string password)
        {
            _username = username;
            _password = password;
        }

        public string GetUsername()
        {
            return _username;
        }

        public string GetPassword()
        {
            return _password;
        }

        /// <summary>
        /// Pokusí se uživatele přihlásit do systému a uloží si vrácené cookie pro budoucí požadavky
        /// </summary>
        /// <exception cref="LoginErrorException">
        /// V případě chyby sítě nebo špatného hesla vyhodí LoginErrorException
        /// </exception>
        public async Task Login()
        {
            // vytvoření správného požadavku, jež UIS očekává
            var content = CzuRequestContentFactory.GetAuth(_username, _password);

            try
            {
                var response = await _httpClient.PostAsync(new Uri($"{UisUri}/system/login.pl"), content);

                // pokud jsme nedostali žádné cookie, vyhodíme výjimku
                if (_cookieContainer.Count < 1)
                {
                    throw new LoginErrorException();
                }
            }
            catch (Exception e)
            {
                throw new LoginErrorException();
            }
        }

        /// <summary>
        /// Získá html string strany rozvrhu dle zadaných parametrů
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
        /// Task vracející html string strany rozvrhu zadané místnosti
        /// </returns>
        public async Task<string> GetRoomPage(ClassRoom room, DateTime from, DateTime to, DayOfWeek dayOfWeek)
        {
            // vytvoření správného požadavku, jež UIS očekává
            var content = CzuRequestContentFactory.GetRoom(room, dayOfWeek, from, to);

            var response =
                await _httpClient.PostAsync(new Uri($"{UisUri}/auth/katalog/rozvrhy_view.pl"), content);

            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Získá html string strany "Zobrazení a tisk rozvrhů"
        /// </summary>
        /// <returns>
        /// Task vracející html string strany s výběrem kritérií pro zobrazení rozvrhů
        /// </returns>
        public async Task<string> GetEnumsPage()
        {
            var response = await _httpClient.GetAsync(new Uri("https://is.czu.cz/auth/katalog/rozvrhy_view.pl"));

            return await response.Content.ReadAsStringAsync();
        }
    }
}