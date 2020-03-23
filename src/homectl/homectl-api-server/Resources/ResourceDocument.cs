using Newtonsoft.Json.Linq;

namespace homectl_api_server.Resources
{
	/// <summary>
	/// Resource document backed by a JSON object.
	/// </summary>
	public abstract class ResourceDocument<T>
		where T : ResourceDocument<T>, new()
	{
		private readonly static JToken _emptyJson = new JObject();

		public static T FromJson(JToken jsonObject)
		{
			return new T { _json = jsonObject };
		}

		private JToken _json = _emptyJson;
	}
}
