using System.Threading;
using System.Threading.Tasks;

namespace HomeCtl.Connection
{
	/// <summary>
	/// Provides a connection to an endpoint.
	/// </summary>
	public interface IConnectionProvider
	{
		/// <summary>
		/// Attempt to connect.
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task<ConnectionResult> AttemptConnection(CancellationToken cancellationToken);
	}
}
