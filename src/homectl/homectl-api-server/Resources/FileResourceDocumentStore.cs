using HomeCtl.Kinds;
using HomeCtl.Kinds.Resources;
using HomeCtl.Resources;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace HomeCtl.ApiServer.Resources
{
	class FileResourceDocumentStore<TKind> : IResourceDocumentStore<TKind>
		where TKind : class, IResource
	{
		private readonly DirectoryInfo _storageDirectory = new DirectoryInfo($"resourceStore/{typeof(TKind).Name}");

		private void EnsureDirectoryExists()
		{
			if (!_storageDirectory.Exists)
				_storageDirectory.Create();
		}

		public Task Store(string key, ResourceDocument resourceDocument)
		{
			EnsureDirectoryExists();

			var filePath = Path.Combine(_storageDirectory.FullName, $"{key}.json");
			var json = ResourceDocumentSerializer.SerializeToJson(resourceDocument);
			return File.WriteAllTextAsync(filePath, json, Encoding.UTF8);
		}
	}
}
