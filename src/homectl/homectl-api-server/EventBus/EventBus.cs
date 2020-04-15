using System;
using System.Collections.Generic;

namespace HomeCtl.ApiServer.EventBus
{
	class EventBus
	{
		private readonly object _lockObj = new object();
		private readonly Dictionary<Type, SubscriptionCollection> _subscriptionCollections =
			new Dictionary<Type, SubscriptionCollection>();

		public void Publish<TEvent>(TEvent eventData)
		{
			SubscriptionCollection<TEvent>? subscriptionCollection;

			lock (_lockObj)
			{
				if (!TryGetSubscriptionCollection<TEvent>(false, out subscriptionCollection) ||
					subscriptionCollection == null)
					return;
			}
			
			subscriptionCollection.Invoke(eventData);
		}

		public void Subscribe<TEvent>(Action<TEvent> @delegate)
		{
			lock (_lockObj)
			{
				TryGetSubscriptionCollection<TEvent>(true, out var subscriptionCollection);
				if (subscriptionCollection == null)
					throw new NullReferenceException();
				subscriptionCollection.Add(@delegate);
			}
		}

		public void Unsubscribe<TEvent>(Action<TEvent> @delegate)
		{
			lock (_lockObj)
			{
				if (!TryGetSubscriptionCollection<TEvent>(false, out var subscriptionCollection) ||
					subscriptionCollection == null)
					return;

				if (subscriptionCollection.Remove(@delegate) < 1)
					RemoveSubscriptionCollection<TEvent>();
			}
		}

		private void RemoveSubscriptionCollection<TEvent>()
		{
			_subscriptionCollections.Remove(typeof(TEvent));
		}

		private bool TryGetSubscriptionCollection<TEvent>(bool createIfNotExists, out SubscriptionCollection<TEvent>? subscriptionCollection)
		{
			var eventType = typeof(TEvent);
			if (!_subscriptionCollections.TryGetValue(eventType, out var foundCollection))
			{
				if (!createIfNotExists)
				{
					subscriptionCollection = null;
					return false;
				}

				subscriptionCollection = new SubscriptionCollection<TEvent>();
				_subscriptionCollections.Add(eventType, subscriptionCollection);
				return true;
			}

			subscriptionCollection = (SubscriptionCollection<TEvent>)foundCollection;
			return true;
		}

		private abstract class SubscriptionCollection
		{
		}

		private class SubscriptionCollection<TEvent> : SubscriptionCollection
		{
			private readonly object _lockObj = new object();
			private readonly List<Action<TEvent>> _subscribers = new List<Action<TEvent>>();

			public int Count
			{
				get
				{
					lock (_lockObj)
						return _subscribers.Count;
				}
			}

			public void Add(Action<TEvent> @delegate)
			{
				lock (_lockObj)
				{
					_subscribers.Add(@delegate);
				}
			}

			public int Remove(Action<TEvent> @delegate)
			{
				lock (_lockObj)
				{
					_subscribers.Remove(@delegate);
					return _subscribers.Count;
				}
			}

			public void Invoke(TEvent eventData)
			{
				lock (_lockObj)
				{
					foreach (var @delegate in _subscribers)
					{
						try
						{
							@delegate(eventData);
						}
						catch (Exception ex)
						{
							//  todo: logging
						}
					}
				}
			}
		}
	}
}
