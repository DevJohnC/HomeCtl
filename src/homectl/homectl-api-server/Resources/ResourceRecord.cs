using System;

namespace HomeCtl.ApiServer.Resources
{
	struct ResourceRecord
	{
		public ResourceRecord(Guid id, string? label = null)
		{
			Id = id;
			Label = label;
		}

		public Guid Id { get; }

		public string? Label { get; }
	}
}
