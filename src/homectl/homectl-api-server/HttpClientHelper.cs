using System;
using System.Net.Http;

namespace HomeCtl.ApiServer
{
	public class HttpClientHelper
	{
		private const string SWITCH_NAME = "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport";

		public static HttpClient CreateHttpClient()
		{
			//AppContext.TryGetSwitch(SWITCH_NAME, out var currentSwitchValue);
			//if (currentSwitchValue == false)
				AppContext.SetSwitch(SWITCH_NAME, true);

			var client = new HttpClient
			{
				DefaultRequestVersion = new Version(2, 0)
			};

			//if (currentSwitchValue == false)
			//	AppContext.SetSwitch(SWITCH_NAME, false);

			return client;
		}
	}
}
