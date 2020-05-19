using Grpc.Core.Logging;
using HomeCtl.Kinds;
using HomeCtl.Kinds.Resources;
using HomeCtl.Resources;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace HomeCtl.ApiServer.Resources
{
	class FileResourceDocumentStore<TKind> : IResourceDocumentStore<TKind>
		where TKind : class, IResource
	{
		private readonly static ResourceDocument[] _empty = new ResourceDocument[0];

		private readonly DirectoryInfo _storageDirectory = new DirectoryInfo($"resourceStore/{typeof(TKind).Name}");
		private readonly ILogger<FileResourceDocumentStore<TKind>> _logger;

		public FileResourceDocumentStore(ILogger<FileResourceDocumentStore<TKind>> logger)
		{
			_logger = logger;
		}

		private void EnsureDirectoryExists()
		{
			if (!_storageDirectory.Exists)
				_storageDirectory.Create();
		}

		private async Task<ResourceDocument?> LoadFromFile(string filePath)
		{
			try
			{
				var json = await File.ReadAllTextAsync(filePath, Encoding.UTF8);
				return ResourceDocumentSerializer.DeserializeFromJson(json);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Failed to load resource from '{filePath}'.");
				return null;
			}
		}

		public async Task<IReadOnlyList<ResourceDocument>> LoadAll()
		{
			if (!_storageDirectory.Exists)
				return _empty;

			var result = new List<ResourceDocument>();

			foreach (var file in _storageDirectory.GetFiles("*.json"))
			{
				var doc = await LoadFromFile(file.FullName);
				if (doc != null)
					result.Add(doc);
			}

			return result;
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
