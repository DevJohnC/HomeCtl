using System;

namespace HomeCtl.Clients
{
	public class Intent
	{
		public string? Action { get; set; }

		public string? Category { get; set; }

		public Uri? DataUri { get; set; }
	}
}
