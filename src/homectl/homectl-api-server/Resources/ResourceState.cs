using Newtonsoft.Json.Linq;

namespace homectl.Resources
{
	public class ResourceState : ResourceDocument<ResourceState>
	{
		public static readonly ResourceState Nothing = new ResourceState();

		protected override void PopulateMembers(JToken jsonObject)
		{
		}
	}
}
