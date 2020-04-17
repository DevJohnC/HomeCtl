using Grpc.Core;
using HomeCtl.Servers.ApiServer;
using System.Threading.Tasks;

namespace HomeCtl.ApiServer.ProtocolServices
{
	class RecordsService : Records.RecordsBase
	{
		public override Task<StoreRecordResponse> StoreRecord(StoreRecordRequest request, ServerCallContext context)
		{
			var resourceDocument = Resources.ResourceDocument.FromProto(request.ResourceRecord);

			return base.StoreRecord(request, context);
		}
	}
}
