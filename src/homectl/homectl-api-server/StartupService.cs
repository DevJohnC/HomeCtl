namespace HomeCtl.ApiServer
{
	internal class StartupService
	{
		public StartupService(object service)
		{
			Service = service;
		}

		public object Service { get; }
	}
}
