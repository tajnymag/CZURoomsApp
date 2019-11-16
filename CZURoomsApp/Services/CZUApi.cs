using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CZURoomsApp.Models;

namespace CZURoomsApp.Services
{
	public class CZUApi
	{
		public readonly Uri UisUri = new Uri("https://is.czu.cz/");

		private readonly CookieContainer _cookieContainer;
		private readonly HttpClient _httpClient;

		private string _password = null;
		private string _username = null;

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

		public async Task Login()
		{
			var content = CZURequestContentFactory.GetAuth(_username, _password);

			try
			{
				var response = await _httpClient.PostAsync(new Uri("https://is.czu.cz/system/login.pl"),
					new FormUrlEncodedContent(content));
			}
			catch (Exception e)
			{
				Console.Error.WriteLine("Nastala chyba při přihlašování do UIS. Zkontrolujte přihlašovací údaje!");
			}
		}
		
		public async Task<string> GetRoomPage(ClassRoom room, DateTime from, DateTime to, DayOfWeek dayOfWeek)
		{
			var content = CZURequestContentFactory.GetRoom(room, dayOfWeek, from, to);

			try
			{
				var response = await _httpClient.PostAsync(new Uri("https://is.czu.cz/auth/katalog/rozvrhy_view.pl"),
					new FormUrlEncodedContent(content));

				return await response.Content.ReadAsStringAsync();
			}
			catch (Exception e)
			{
				Console.Error.WriteLine("Nepodařilo se načíst stránku místnosti!");
				return null;
			}
		}
	}
}