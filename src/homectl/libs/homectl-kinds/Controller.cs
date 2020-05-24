namespace HomeCtl.Kinds
{
	public class Controller : IResource
	{
		public Kind Kind => CoreKinds.Controller;

		public string GetIdentity()
		{
			throw new System.NotImplementedException();
		}
	}
}
