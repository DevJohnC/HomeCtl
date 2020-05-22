using HomeCtl.Kinds;
using HomeCtl.Resources;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace homectl_kinds_tests
{
	[TestClass]
	public class HostTests
	{
		private readonly string _testHostJson = @"{
  ""kind"": {
    ""kindName"": ""host"",
    ""group"": ""core"",
    ""apiVersion"": ""v1alpha1""
  },
  ""definition"": {
    ""hostId"": ""8ccaed32-c0b6-4767-b8dc-683c07118eec"",
    ""endpoint"": ""127.0.0.1""
  },
  ""metadata"": {
    ""machineName"": ""TestHost""
  },
  ""state"": {
    ""connectedState"": ""Connected""
  }
}";

		[TestMethod]
		public void Can_Serialize_To_Json()
		{
			var host = new Host
			{
				HostId = Guid.Parse("{8CCAED32-C0B6-4767-B8DC-683C07118EEC}"),
				MachineName = "TestHost",
				Endpoint = "127.0.0.1"
			};

			var json = Resources.SaveToString(host);

			Assert.AreEqual(_testHostJson, json);
		}

		[TestMethod]
		public void Can_Deserialize_To_Host()
		{
			var host = Resources.LoadFromString(CoreKinds.Host, _testHostJson);

			Assert.IsNotNull(host);
			Assert.ReferenceEquals(CoreKinds.Host, host.Kind);
			Assert.AreEqual(Guid.Parse("{8CCAED32-C0B6-4767-B8DC-683C07118EEC}"), host.HostId);
			Assert.AreEqual("TestHost", host.MachineName);
			Assert.AreEqual("127.0.0.1", host.Endpoint);
		}
	}
}
