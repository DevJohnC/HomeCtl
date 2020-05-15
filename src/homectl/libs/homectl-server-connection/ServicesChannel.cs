using Grpc.Core;
using HomeCtl.Events;
using System;
using System.Threading.Tasks;

namespace HomeCtl.Connection
{
	public static class ServicesChannelEvents
	{
		public class ExceptionThrown
		{
			public ExceptionThrown(ServerEndpoint serverEndpoint, Exception exception)
			{
				ServerEndpoint = serverEndpoint;
				Exception = exception;
			}

			public ServerEndpoint ServerEndpoint { get; }

			public Exception Exception { get; }
		}
	}

	public class ServicesChannel : ChannelBase
	{
		private readonly EventBus _eventBus;
		private readonly ServerEndpoint _serverEndpoint;
		private readonly ChannelBase _proxiedChannel;

		public ServicesChannel(EventBus eventBus, ServerEndpoint serverEndpoint, ChannelBase proxiedChannel) :
			base(proxiedChannel.Target)
		{
			_eventBus = eventBus;
			_serverEndpoint = serverEndpoint;
			_proxiedChannel = proxiedChannel;
		}

		public override CallInvoker CreateCallInvoker()
		{
			var impl = _proxiedChannel.CreateCallInvoker();
			var monitoredInvoker = new MonitoredCallInvoker(_eventBus, _serverEndpoint, impl);
			return monitoredInvoker;
		}

		protected override async Task ShutdownAsyncCore()
		{
			await base.ShutdownAsyncCore();
			await _proxiedChannel.ShutdownAsync();
		}

		private class MonitoredCallInvoker : CallInvoker
		{
			private readonly EventBus _eventBus;
			private readonly ServerEndpoint _serverEndpoint;
			private readonly CallInvoker _impl;

			public MonitoredCallInvoker(EventBus eventBus, ServerEndpoint serverEndpoint, CallInvoker impl)
			{
				_eventBus = eventBus;
				_serverEndpoint = serverEndpoint;
				_impl = impl;
			}

			private void HandleException(Exception ex)
			{
				_eventBus.Publish(
					new ServicesChannelEvents.ExceptionThrown(_serverEndpoint, ex)
					);
			}

			public override AsyncClientStreamingCall<TRequest, TResponse> AsyncClientStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options)
			{
				try
				{
					return _impl.AsyncClientStreamingCall(method, host, options);
				}
				catch (Exception ex)
				{
					HandleException(ex);
					throw;
				}
			}

			public override AsyncDuplexStreamingCall<TRequest, TResponse> AsyncDuplexStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options)
			{
				try
				{
					//  todo: monitor any Tasks in the result for exceptions
					return _impl.AsyncDuplexStreamingCall(method, host, options);
				}
				catch (Exception ex)
				{
					HandleException(ex);
					throw;
				}
			}

			public override AsyncServerStreamingCall<TResponse> AsyncServerStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
			{
				try
				{
					//  todo: monitor any Tasks in the result for exceptions
					return _impl.AsyncServerStreamingCall(method, host, options, request);
				}
				catch (Exception ex)
				{
					HandleException(ex);
					throw;
				}
			}

			private async Task<TResponse> MonitorResponse<TResponse>(Task<TResponse> originalTask)
			{
				try
				{
					return await originalTask;
				}
				catch (Exception ex)
				{
					HandleException(ex);
					throw;
				}
			}

			public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
			{
				try
				{
					var result = _impl.AsyncUnaryCall(method, host, options, request);
					return new AsyncUnaryCall<TResponse>(
						MonitorResponse(result.ResponseAsync),
						result.ResponseHeadersAsync,
						result.GetStatus,
						result.GetTrailers,
						result.Dispose
						);
				}
				catch (Exception ex)
				{
					HandleException(ex);
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
					HandleException(ex);
					throw;
				}
			}
		}
	}
}
