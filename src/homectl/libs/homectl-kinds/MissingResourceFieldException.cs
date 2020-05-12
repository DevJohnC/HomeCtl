using System;
using System.Runtime.Serialization;

namespace HomeCtl.Kinds
{
	[Serializable]
	public class MissingResourceFieldException : Exception
	{
		public MissingResourceFieldException()
		{
		}

		public MissingResourceFieldException(string fieldName) : base($"Resource was missing field '{fieldName}'")
		{
		}

		public MissingResourceFieldException(string fieldName, Exception innerException) : base($"Resource was missing field '{fieldName}'", innerException)
		{
		}

		protected MissingResourceFieldException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}