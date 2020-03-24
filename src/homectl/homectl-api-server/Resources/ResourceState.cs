using Newtonsoft.Json.Linq;

namespace homectl_api_server.Resources
{
	public class ResourceState : ResourceDocument<ResourceState>
	{
		public static readonly ResourceState Nothing = new ResourceState();

		protected override void PopulateMembers(JToken jsonObject)
		{
		}
	}
}
