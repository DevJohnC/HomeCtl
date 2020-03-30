using Newtonsoft.Json.Linq;

namespace homectl.Resources
{
	public class ResourceMetadata : ResourceDocument<ResourceMetadata>
	{
		public static readonly ResourceMetadata Nothing = new ResourceMetadata();

		protected override void PopulateMembers(JToken jsonObject)
		{
		}
	}
}
