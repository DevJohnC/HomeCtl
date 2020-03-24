using Newtonsoft.Json.Linq;

namespace homectl_api_server.Resources
{
	public class ResourceSpec : ResourceDocument<ResourceSpec>
	{
		public static readonly ResourceSpec Nothing = new ResourceSpec();

		protected override void PopulateMembers(JToken jsonObject)
		{
		}
	}
}
