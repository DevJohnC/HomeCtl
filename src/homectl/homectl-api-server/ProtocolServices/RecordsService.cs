using Grpc.Core;
using HomeCtl.Servers.ApiServer;
using System.Threading.Tasks;

namespace HomeCtl.ApiServer.ProtocolServices
{
	class RecordsService : Records.RecordsBase
	{
		public override Task<StoreRecordResponse> StoreRecord(StoreRecordRequest request, ServerCallContext context)
		{
			return base.StoreRecord(request, context);
		}
	}
}
