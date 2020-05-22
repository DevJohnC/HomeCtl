using HomeCtl.ApiServer.Resources;
using HomeCtl.Kinds;
using HomeCtl.Kinds.Resources;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace homectl_api_server_tests.Resources
{
	[TestClass]
	public class ResourceOrchestratorTests
	{
		[TestMethod]
		public async Task Apply_Creates_Resource_In_Correct_Manager_When_No_Match_Exists()
		{
			var testClassManager = new TestKinds.TestClassManager(new DoNothingResourceStore<TestKinds.TestClass>());
			var resourceOrchestrator = new ResourceOrchestrator(new[]
			{
				testClassManager
			}, new ResourceManagerAccessor());

			var testObjOrigin = new TestKinds.TestClass { Id = Guid.NewGuid() };
			if (!testObjOrigin.Kind.TryConvertToDocument(testObjOrigin, out var resourceDocument))
				Assert.Fail();

			await resourceOrchestrator.Apply(resourceDocument);

			Assert.AreEqual(1, testClassManager.CreateCount);
		}

		[TestMethod]
		public async Task Apply_Updates_Resource_In_Correct_Manager_When_Match_Exists()
		{
			var testClassManager = new TestKinds.TestClassManager(new DoNothingResourceStore<TestKinds.TestClass>());
			var resourceOrchestrator = new ResourceOrchestrator(new[]
			{
				testClassManager
			}, new ResourceManagerAccessor());

			var testObjOrigin = new TestKinds.TestClass { Id = Guid.NewGuid() };
			if (!testObjOrigin.Kind.TryConvertToDocument(testObjOrigin, out var resourceDocument))
				Assert.Fail();

			await resourceOrchestrator.Apply(resourceDocument);
			await resourceOrchestrator.Apply(resourceDocument);

			Assert.AreEqual(1, testClassManager.UpdateCount);
		}

		private class DoNothingResourceStore<T> : IResourceDocumentStore<T>
			where T : class, IResource
		{
			public Task<IReadOnlyList<ResourceDocument>> LoadAll()
			{
				throw new NotImplementedException();
			}

			public Task Store(string key, ResourceDocument resourceDocument)
			{
				throw new NotImplementedException();
			}
		}

		private static class TestKinds
		{
			public static readonly Kind<TestClass> TestClassKind = KindBuilder.Build(
				"tests", "testing", "testClass", "testClasses",
				TestClass.ConvertToDocument, TestClass.ConvertToResource,
				metadata => metadata.RequireString("id"),
				definition => definition.RequireString("identity")
				);

			public class TestClassManager : ResourceManager<TestClass>
			{
				public TestClassManager(IResourceDocumentStore<TestClass> documentStore) : base(documentStore)
				{
				}

				protected override Kind<TestClass> TypedKind => TestClassKind;

				public int CreateCount { get; private set; }

				public int UpdateCount { get; private set; }

				public override Task CreateResource(ResourceDocument resourceDocument)
				{
					CreateCount++;
					return base.CreateResource(resourceDocument);
				}

				public override Task UpdateResource(IResource resource, ResourceDocument resourceDocument)
				{
					UpdateCount++;
					return base.UpdateResource(resource, resourceDocument);
				}

				protected override bool TryGetKey(ResourceDocument resourceDocument, [NotNullWhen(true)] out string key)
				{
					key = resourceDocument.Metadata["id"].GetString();
					return true;
				}
			}

			public class TestClass : IResource
			{
				public Guid Id { get; set; }

				public Kind Kind => TestClassKind;

				public static ResourceDocument ConvertToDocument(TestClass obj)
				{
					return new ResourceDocument(
						TestClassKind.GetKindDescriptor(),
						new ResourceMetadata(new List<ResourceField>
						{
							new ResourceField("id", ResourceFieldValue.String(obj.Id.ToString()))
						}),
						new ResourceDefinition(new List<ResourceField>
						{
							new ResourceField("identity", ResourceFieldValue.String(obj.Id.ToString()))
						}));
				}

				public static TestClass ConvertToResource(ResourceDocument document)
				{
					return new TestClass
					{
						Id = Guid.Parse(document.Metadata["id"].GetString())
					};
				}
			}
		}
	}
}
