using System.Threading.Tasks;

namespace homectl.Controllers
{
	public abstract class Controller
	{
		public abstract Task<SpecChangeResponse> ApplySpecChange(SpecChangeRequest changeRequest);
	}
}
