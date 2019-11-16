using System.Collections.Generic;

namespace CZURoomsApp.Models
{
	public class CZUAuthRequestContent
	{
		private string _loginHidden;
		private string _destination;
		private string _authIdHidden;
		private string _credential0;
		private string _credential1;
		private string _credential2;
		private string _credentialK;

		public CZUAuthRequestContent(string loginHidden, string destination, string authIdHidden, string credential0,
			string credential1, string credential2, string credentialK)
		{
			_loginHidden = loginHidden;
			_destination = destination;
			_authIdHidden = authIdHidden;
			_credential0 = credential0;
			_credential1 = credential1;
			_credential2 = credential2;
			_credentialK = credentialK;
		}

		public KeyValuePair<string, string>[] ToKeyValuePairs()
		{
			return new[]
			{
				new KeyValuePair<string, string>("login_hidden", _loginHidden),
				new KeyValuePair<string, string>("destination", _destination),
				new KeyValuePair<string, string>("auth_id_hidden", _authIdHidden),
				new KeyValuePair<string, string>("credential_0", _credential0),
				new KeyValuePair<string, string>("credential_1", _credential1),
				new KeyValuePair<string, string>("credential_2", _credential2),
				new KeyValuePair<string, string>("credential_k", _credentialK)
			};
		}
	}
}