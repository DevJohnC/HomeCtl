using System.Threading.Tasks;

namespace homectl_api_client.Controllers
{
	public abstract class Controller
	{
		public abstract Task<SpecChangeResponse> ApplySpecChange(SpecChangeRequest changeRequest);
	}
}
