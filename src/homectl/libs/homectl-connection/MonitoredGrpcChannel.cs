using Grpc.Core;
using System;
using System.Threading.Tasks;

namespace HomeCtl.Connection
{
	/// <summary>
	/// Contains event data for a call invoker error.
	/// </summary>
	class CallInvokerErrorEventArgs : EventArgs
	{
		public CallInvokerErrorEventArgs(Exception exception)
		{
			Exception = exception;
		}

		public Exception Exception { get; }
	}

	/// <summary>
	/// A monitored grpc channel that will notify the connection manager when a request errors.
	/// </summary>
	sealed class MonitoredGrpcChannel : ChannelBase
	{
		private readonly ChannelBase _impl;

		public EventHandler<CallInvokerErrorEventArgs>? InvokeError;

		public MonitoredGrpcChannel(ChannelBase impl) : base(impl.Target)
		{
			_impl = impl;
		}

		public override CallInvoker CreateCallInvoker()
		{
			var impl = _impl.CreateCallInvoker();
			var monitoredInvoker = new MonitoredCallInvoker(impl);
			monitoredInvoker.InvokeError += (s, e) =>
				InvokeError?.Invoke(this, e);
			return monitoredInvoker;
		}

		protected override async Task ShutdownAsyncCore()
		{
			await base.ShutdownAsyncCore();
			await _impl.ShutdownAsync();
		}

		private class MonitoredCallInvoker : CallInvoker
		{
			private readonly CallInvoker _impl;

			public EventHandler<CallInvokerErrorEventArgs>? InvokeError;

			public MonitoredCallInvoker(CallInvoker impl)
			{
				_impl = impl;
			}

			public override AsyncClientStreamingCall<TRequest, TResponse> AsyncClientStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options)
			{
				try
				{
					return _impl.AsyncClientStreamingCall(method, host, options);
				}
				catch (Exception ex)
				{
					InvokeError?.Invoke(this, new CallInvokerErrorEventArgs(ex));
					throw;
				}
			}

			public override AsyncDuplexStreamingCall<TRequest, TResponse> AsyncDuplexStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options)
			{
				try
				{
					return _impl.AsyncDuplexStreamingCall(method, host, options);
				}
				catch (Exception ex)
				{
					InvokeError?.Invoke(this, new CallInvokerErrorEventArgs(ex));
					throw;
				}
			}

			public override AsyncServerStreamingCall<TResponse> AsyncServerStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
			{
				try
				{
					return _impl.AsyncServerStreamingCall(method, host, options, request);
				}
				catch (Exception ex)
				{
					InvokeError?.Invoke(this, new CallInvokerErrorEventArgs(ex));
					throw;
				}
			}

			public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
			{
				try
				{
					return _impl.AsyncUnaryCall(method, host, options, request);
				}
				catch (Exception ex)
				{
					InvokeError?.Invoke(this, new CallInvokerErrorEventArgs(ex));
					throw;
				}
			}

			public override TResponse BlockingUnaryCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
			{
				try
				{
					return _impl.BlockingUnaryCall(method, host, options, request);
				}
				catch (Exception ex)
				{
					InvokeError?.Invoke(this, new CallInvokerErrorEventArgs(ex));
					throw;
				}
			}
		}
	}
}
