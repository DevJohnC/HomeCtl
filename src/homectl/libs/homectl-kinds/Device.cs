namespace HomeCtl.Kinds
{
	public class Device : IResource
	{
		public Kind Kind => CoreKinds.Device;

		public string GetIdentity()
		{
			throw new System.NotImplementedException();
		}
	}
}
