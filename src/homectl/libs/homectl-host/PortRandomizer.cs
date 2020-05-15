using System;
using System.Net;
using System.Net.Sockets;

namespace HomeCtl.Host
{
	class PortRandomizer
	{
		public static int GetRandomPort()
		{
			while (true)
			{
				var rng = new Random();
				var portNumber = rng.Next(1025, 65535);
				if (!TestPort(portNumber, IPAddress.Any) ||
					!TestPort(portNumber, IPAddress.IPv6Any))
					continue;

				return portNumber;
			}
		}

		private static bool TestPort(int portNumber, IPAddress ipAddress)
		{
			try
			{
				using (var socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
				{
					socket.Bind(new IPEndPoint(ipAddress, portNumber));
					return true;
				}
			}
			catch
			{
				return false;
			}
		}
	}
}
