using System;

namespace homectl.Resources
{
	public class ResourceRecord
	{
		public ResourceRecord(Guid id)
		{
			Id = id;
		}

		public Guid Id { get; }
	}
}
