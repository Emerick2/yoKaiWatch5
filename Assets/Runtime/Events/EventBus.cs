using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace YKW.Core.Events
{
    /// <summary>A synchronous, strongly typed event service shared by all scenes.</summary>
    public static class EventBus
    {
        private interface IChannel
        {
            void Clear();
            void PublishObject(object eventData);
        }

        private sealed class Channel<TEvent> : IChannel
        {
            private readonly List<Action<TEvent>> handlers = new List<Action<TEvent>>();
            private readonly List<Action<TEvent>> pendingAdd = new List<Action<TEvent>>();
            private readonly List<Action<TEvent>> pendingRemove = new List<Action<TEvent>>();
            private int dispatchDepth;

            public IDisposable Subscribe(Action<TEvent> handler)
            {
                if (handler == null) throw new ArgumentNullException(nameof(handler));
                bool alreadySubscribed = Contains(handler);
                if (alreadySubscribed && dispatchDepth > 0) pendingRemove.Remove(handler);
                if (!alreadySubscribed)
                {
                    if (dispatchDepth == 0) handlers.Add(handler);
                    else pendingAdd.Add(handler);
                }
                return new Subscription<TEvent>(this, handler, !alreadySubscribed);
            }

            public void Unsubscribe(Action<TEvent> handler)
            {
                if (handler == null) return;
                if (dispatchDepth == 0) handlers.Remove(handler);
                else if (!pendingRemove.Contains(handler)) pendingRemove.Add(handler);
            }

            public void Publish(TEvent eventData)
            {
                dispatchDepth++;
                List<Exception> failures = null;
                int count = handlers.Count;
                try
                {
                    for (int i = 0; i < count; i++)
                    {
                        try { handlers[i](eventData); }
                        catch (Exception exception)
                        {
                            if (failures == null) failures = new List<Exception>();
                            failures.Add(exception);
                        }
                    }
                }
                finally
                {
                    dispatchDepth--;
                    if (dispatchDepth == 0) ApplyPending();
                }

                if (failures != null)
                    throw new AggregateException("One or more event handlers failed for " + typeof(TEvent).FullName, failures);
            }

            private bool Contains(Action<TEvent> handler)
            {
                return handlers.Contains(handler) || pendingAdd.Contains(handler);
            }

            private void ApplyPending()
            {
                for (int i = 0; i < pendingRemove.Count; i++)
                {
                    handlers.Remove(pendingRemove[i]);
                    pendingAdd.Remove(pendingRemove[i]);
                }
                pendingRemove.Clear();
                for (int i = 0; i < pendingAdd.Count; i++)
                    if (!handlers.Contains(pendingAdd[i])) handlers.Add(pendingAdd[i]);
                pendingAdd.Clear();
            }

            public void Clear()
            {
                handlers.Clear();
                pendingAdd.Clear();
                pendingRemove.Clear();
                dispatchDepth = 0;
            }

            public void PublishObject(object eventData) => Publish((TEvent)eventData);
        }

        private sealed class Subscription<TEvent> : IDisposable
        {
            private Channel<TEvent> channel;
            private Action<TEvent> handler;
            private readonly bool ownsRegistration;

            public Subscription(Channel<TEvent> channel, Action<TEvent> handler, bool ownsRegistration)
            {
                this.channel = channel;
                this.handler = handler;
                this.ownsRegistration = ownsRegistration;
            }

            public void Dispose()
            {
                if (channel == null) return;
                if (ownsRegistration) channel.Unsubscribe(handler);
                channel = null;
                handler = null;
            }
        }

        private static readonly Dictionary<Type, IChannel> channels = new Dictionary<Type, IChannel>();
        private static SynchronizationContext unityContext;
        private static int unityThreadId;
        internal static event Action StaticStateReset;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetForPlayMode()
        {
            foreach (IChannel channel in channels.Values) channel.Clear();
            channels.Clear();
            unityContext = null;
            unityThreadId = 0;
            Action reset = StaticStateReset;
            if (reset != null) reset();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void CaptureUnityThread()
        {
            unityContext = SynchronizationContext.Current;
            unityThreadId = Thread.CurrentThread.ManagedThreadId;
        }

        public static IDisposable Subscribe<TEvent>(Action<TEvent> handler)
        {
            EnsureMainThread();
            return GetChannel<TEvent>().Subscribe(handler);
        }

        public static void Unsubscribe<TEvent>(Action<TEvent> handler)
        {
            EnsureMainThread();
            GetChannel<TEvent>().Unsubscribe(handler);
        }

        public static void Publish<TEvent>(TEvent eventData)
        {
            EnsureMainThread();
            Channel<TEvent> channel = GetChannel<TEvent>(false);
            if (channel != null) channel.Publish(eventData);
        }

        /// <summary>Queues an event for Unity's captured synchronization context.</summary>
        public static void PublishOnMainThread<TEvent>(TEvent eventData)
        {
            if (unityThreadId == 0 && Thread.CurrentThread.ManagedThreadId == 1)
                CaptureUnityThread();
            if (unityThreadId == Thread.CurrentThread.ManagedThreadId)
            {
                Publish(eventData);
                return;
            }
            if (unityContext == null)
                throw new InvalidOperationException("EventBus has not captured Unity's main-thread synchronization context.");
            unityContext.Post(state =>
            {
                try { Publish((TEvent)state); }
                catch (Exception exception) { Debug.LogException(exception); }
            }, eventData);
        }

        internal static IDisposable Subscribe(Type eventType, object target, System.Reflection.MethodInfo method)
        {
            System.Reflection.MethodInfo subscribe = typeof(EventBus).GetMethod(
                nameof(SubscribeMethod), System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            return (IDisposable)subscribe.MakeGenericMethod(eventType).Invoke(null, new object[] { target, method });
        }

        internal static void PublishObject(Type eventType, object eventData)
        {
            EnsureMainThread();
            IChannel channel;
            if (channels.TryGetValue(eventType, out channel)) channel.PublishObject(eventData);
        }

        internal static void PublishObjectOnMainThread(Type eventType, object eventData)
        {
            if (eventType == null) throw new ArgumentNullException(nameof(eventType));
            if (unityThreadId == 0 && Thread.CurrentThread.ManagedThreadId == 1)
                CaptureUnityThread();
            if (Thread.CurrentThread.ManagedThreadId == unityThreadId)
            {
                PublishObject(eventType, eventData);
                return;
            }
            if (unityContext == null)
                throw new InvalidOperationException("EventBus has not captured Unity's main-thread synchronization context.");

            unityContext.Post(_ =>
            {
                try { PublishObject(eventType, eventData); }
                catch (Exception exception) { Debug.LogException(exception); }
            }, null);
        }

        private static IDisposable SubscribeMethod<TEvent>(object target, System.Reflection.MethodInfo method)
        {
            Action<TEvent> callback = (Action<TEvent>)Delegate.CreateDelegate(typeof(Action<TEvent>), target, method);
            return Subscribe(callback);
        }

        private static Channel<TEvent> GetChannel<TEvent>(bool create = true)
        {
            Type eventType = typeof(TEvent);
            IChannel channel;
            if (channels.TryGetValue(eventType, out channel)) return (Channel<TEvent>)channel;
            if (!create) return null;
            Channel<TEvent> created = new Channel<TEvent>();
            channels.Add(eventType, created);
            return created;
        }

        private static void EnsureMainThread()
        {
            if (unityThreadId == 0)
            {
                unityThreadId = Thread.CurrentThread.ManagedThreadId;
                unityContext = SynchronizationContext.Current;
            }
            if (Thread.CurrentThread.ManagedThreadId != unityThreadId)
                throw new InvalidOperationException("Synchronous EventBus.Publish must run on Unity's main thread. Use PublishOnMainThread from workers.");
        }
    }
}
