using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace homectl.Resources
{
	public interface IExpandoDocument
	{
		Dictionary<string, object> Document { get; }
	}

	/// <summary>
	/// Resource document backed by a JSON object.
	/// </summary>
	public abstract class ResourceDocument<T> : IExpandoDocument
		where T : ResourceDocument<T>, new()
	{
		public static T FromJson(JToken jsonObject)
		{
			//var obj = new T { _document = jsonObject };
			//obj.PopulateMembers(jsonObject);
			//return obj;
			return default;
		}

		private Dictionary<string, object> _document = new Dictionary<string, object>();

		Dictionary<string, object> IExpandoDocument.Document => Document;

		protected Dictionary<string, object> Document => _document;
	}
}
