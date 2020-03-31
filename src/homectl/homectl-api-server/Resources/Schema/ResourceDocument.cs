using Newtonsoft.Json.Linq;

namespace homectl.Resources
{
	public interface IJsonDocument
	{
		JToken Json { get; }
	}

	/// <summary>
	/// Resource document backed by a JSON object.
	/// </summary>
	public abstract class ResourceDocument<T> : IJsonDocument
		where T : ResourceDocument<T>, new()
	{
		public static T FromJson(JToken jsonObject)
		{
			var obj = new T { _json = jsonObject };
			obj.PopulateMembers(jsonObject);
			return obj;
		}

		private JToken _json = new JObject();

		JToken IJsonDocument.Json => Json;

		protected JToken Json => _json;

		protected abstract void PopulateMembers(JToken jsonObject);
	}
}
