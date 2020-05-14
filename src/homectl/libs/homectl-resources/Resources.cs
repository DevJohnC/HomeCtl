using HomeCtl.Kinds;
using System.IO;
using System.Text;

namespace HomeCtl.Resources
{
	public class Resources
	{
		public static T? LoadFromFile<T>(Kind<T> kind, string filePath)
			where T : class, IResource
		{
			var resourceDocumentJson = File.ReadAllText(filePath, Encoding.UTF8);
			return LoadFromString(kind, resourceDocumentJson);
		}

		public static T? LoadFromString<T>(Kind<T> kind, string documentStr)
			where T : class, IResource
		{
			var resourceDocument = ResourceDocumentSerializer.DeserializeFromJson(documentStr);
			if (resourceDocument == null)
				throw new System.Exception("Failed to deserialize resource document.");

			if (!kind.TryConvertToResourceInstance(resourceDocument, out T? instance))
				return default;
			return instance;
		}

		public static void SaveToFile(IResource resource, string filePath)
		{
			var resourceDocumentJson = SaveToString(resource);
			File.WriteAllText(filePath, resourceDocumentJson);
		}

		public static string SaveToString(IResource resource)
		{
			if (!resource.Kind.TryConvertToDocument(resource, out var resourceDocument) ||
				resourceDocument == null)
			{
				throw new System.Exception("Failed to convert resource to document.");
			}

			return ResourceDocumentSerializer.SerializeToJson(resourceDocument);
		}
	}
}
