using System;

namespace HomeCtl.Connection
{
	public struct NetworkTiming
	{
		public NetworkTiming(TimeSpan roundtripTime, TimeSpan txTime) : this()
		{
			RoundtripTime = roundtripTime;
			TxTime = txTime;
			RxTime = RoundtripTime - TxTime;
		}

		public TimeSpan RoundtripTime { get; }

		public TimeSpan TxTime { get; }

		public TimeSpan RxTime { get; }
	}
}
