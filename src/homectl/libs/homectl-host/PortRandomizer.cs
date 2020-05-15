using System;

namespace HomeCtl.Host
{
	class PortRandomizer
	{
		public static int GetRandomPort()
		{
			//  todo: bind to the random port to ensure the port is available
			var rng = new Random();
			return rng.Next(1025, 65535);
		}
	}
}
