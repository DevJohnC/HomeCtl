using Newtonsoft.Json.Linq;

namespace homectl.Resources
{
	public class ResourceSpec : ResourceDocument<ResourceSpec>
	{
		public static readonly ResourceSpec Nothing = new ResourceSpec();

		protected override void PopulateMembers(JToken jsonObject)
		{
		}
	}
}
