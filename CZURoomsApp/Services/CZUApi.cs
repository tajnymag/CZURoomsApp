using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CZURoomsApp.Models;
using Eto.Forms;

namespace CZURoomsApp.Services
{
    public class LoginErrorException : Exception
    {
        public LoginErrorException()
        {
        }

        public LoginErrorException(string message) : base(message)
        {
        }

        public LoginErrorException(string message, Exception inner) : base(message, inner)
        {
        }
    }

    public class CZUApi
    {
        public readonly Uri UisUri = new Uri("https://is.czu.cz/");

        private readonly CookieContainer _cookieContainer;
        private readonly HttpClient _httpClient;

        private string _username = null;
        private string _password = null;

        private CZUApi()
        {
            _cookieContainer = new CookieContainer();
            _httpClient = new HttpClient(new HttpClientHandler {CookieContainer = _cookieContainer});

            _httpClient.DefaultRequestHeaders.Add("Accept-Language", "cs,en;q=0.5");
        }

        public CZUApi(string username, string password) : this()
        {
            _username = username;
            _password = password;
        }

        public CZUApi(Cookie cookie) : this()
        {
            _cookieContainer.Add(new Uri(cookie.Path), cookie);
        }

        public void UpdateCredentials(string username, string password)
        {
            _username = username;
            _password = password;
        }

        public async Task Login()
        {
            var content = CZURequestContentFactory.GetAuth(_username, _password);

            try
            {
                var response = await _httpClient.PostAsync(new Uri("https://is.czu.cz/system/login.pl"), content);

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

        public async Task<string> GetRoomPage(ClassRoom room, DateTime from, DateTime to, DayOfWeek dayOfWeek)
        {
            var content = CZURequestContentFactory.GetRoom(room, dayOfWeek, from, to);

            var response =
                await _httpClient.PostAsync(new Uri("https://is.czu.cz/auth/katalog/rozvrhy_view.pl"), content);

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetEnumsPage()
        {
            var response = await _httpClient.GetAsync(new Uri("https://is.czu.cz/auth/katalog/rozvrhy_view.pl"));

            return await response.Content.ReadAsStringAsync();
        }
    }
}