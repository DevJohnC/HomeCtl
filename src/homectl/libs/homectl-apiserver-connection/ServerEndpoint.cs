using System;

namespace HomeCtl.Connection
{
	/// <summary>
	/// An endpoint for a homectl server (api server or host).
	/// </summary>
	public struct ServerEndpoint
	{
		public ServerEndpoint(Uri uri, ServerEndpointIdentityPolicy identityPolicy)
		{
			Uri = uri;
			IdentityPolicy = identityPolicy;
		}

		public Uri Uri { get; }

		public ServerEndpointIdentityPolicy IdentityPolicy { get; }
	}

	public abstract class ServerEndpointIdentityPolicy
	{
		public abstract bool IsValid(ServerIdentityCredentials serverCredentials);
	}

	public class AnyServerIdentityPolicy : ServerEndpointIdentityPolicy
	{
		public static readonly AnyServerIdentityPolicy Instance = new AnyServerIdentityPolicy();

		public override bool IsValid(ServerIdentityCredentials serverCredentials)
		{
			return true;
		}
	}
}
