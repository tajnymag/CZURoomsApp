using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using CZURoomsApp.Models;

namespace CZURoomsApp.Services
{
	public class CZURequestContentFactory
	{
		public CZURequestContentFactory()
		{
		}

		private static HttpContent FormUrlEncodedContent(IEnumerable<KeyValuePair<string, string>> parameters)
		{
			string formUrl = "";

			foreach (var parameter in parameters)
			{
				formUrl += $"{parameter.Key}={parameter.Value}&";
			}

			return new StringContent(formUrl, Encoding.UTF8, "application/x-www-form-urlencoded");
		}
		
		public static HttpContent GetAuth(string username, string password,
			bool loginHidden = true,
			string destination = "/auth", bool authIdHidden = false, int cookieLife = 2073600, string credentialK = "")
		{
			var parameters = new []
			{
				new KeyValuePair<string, string>("login_hidden", loginHidden ? "1" : "0"),
				new KeyValuePair<string, string>("destination", destination),
				new KeyValuePair<string, string>("auth_id_hidden", authIdHidden ? "1" : " 0"),
				new KeyValuePair<string, string>("credential_0", username),
				new KeyValuePair<string, string>("credential_1", password),
				new KeyValuePair<string, string>("credential_2", cookieLife.ToString()),
				new KeyValuePair<string, string>("credential_k", credentialK),
			};
			
			return FormUrlEncodedContent(parameters);
		}

		public static HttpContent GetRoom(ClassRoom room, DayOfWeek dayOfWeek,
			DateTime from, DateTime to, int z = 1,
			int k = 1, int timetable = 944, bool baseNotes = true, bool changes = true, bool pairing = true,
			bool differentTeacher = true,
			bool differentLocation = true, bool otherChanges = true, string displayMode = "konani",
			string format = "list",
			bool reservations = true, bool notes = true, bool show = true, string show2 = "Zobrazit")
		{
			int correctedDayOfWeek = (int) dayOfWeek;
			if (correctedDayOfWeek == 6)
			{
				correctedDayOfWeek = 7;
			}

			string fromDay = from.Day.ToString().PadLeft(2);
			string fromMonth = from.Month.ToString().PadLeft(2);
			string toDay = to.Day.ToString().PadLeft(2);
			string toMonth = to.Month.ToString().PadLeft(2);
			
			string formattedFrom = $"{fromDay}.+{fromMonth}.+{from.Year}";
			string formattedTo = $"{toDay}.+{toMonth}.+{to.Year}";

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

			if (room.Id != 0)
			{
				parameters.Add(new KeyValuePair<string, string>("mistnost", room.Id.ToString()));
			}

			return FormUrlEncodedContent(parameters);
		}
	}
}