using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace YKW.Core.Events
{
    /// <summary>Opt-in lifecycle host for methods marked with [SubscribeEvent].</summary>
    public abstract class EventSubscriberBehaviour : MonoBehaviour
    {
        private sealed class HandlerDescriptor
        {
            public Type EventType;
            public MethodInfo Method;
        }

        private static readonly Dictionary<Type, HandlerDescriptor[]> descriptors =
            new Dictionary<Type, HandlerDescriptor[]>();
        private static readonly List<EventSubscriberBehaviour> activeSubscribers = new List<EventSubscriberBehaviour>();
        private static readonly MethodInfo subscribeGeneric = typeof(EventSubscriberBehaviour).GetMethod(
            nameof(SubscribeTyped), BindingFlags.NonPublic | BindingFlags.Instance);
        private readonly List<IDisposable> subscriptions = new List<IDisposable>();

        static EventSubscriberBehaviour()
        {
            EventBus.StaticStateReset += RebindActiveSubscribers;
        }

        protected virtual void OnEnable()
        {
            if (!activeSubscribers.Contains(this)) activeSubscribers.Add(this);
            RegisterSubscriptions();
        }

        protected virtual void OnDisable()
        {
            activeSubscribers.Remove(this);
            DisposeSubscriptions();
        }

        private void RegisterSubscriptions()
        {
            if (subscriptions.Count != 0) return;
            HandlerDescriptor[] handlers = GetDescriptors(GetType());
            for (int i = 0; i < handlers.Length; i++)
            {
                try
                {
                    subscriptions.Add((IDisposable)subscribeGeneric.MakeGenericMethod(handlers[i].EventType)
                        .Invoke(this, new object[] { handlers[i].Method }));
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception, this);
                    DisposeSubscriptions();
                    return;
                }
            }
        }

        private IDisposable SubscribeTyped<TEvent>(MethodInfo method)
        {
            Action<TEvent> target = (Action<TEvent>)Delegate.CreateDelegate(typeof(Action<TEvent>), this, method);
            Action<TEvent> safeHandler = evt =>
            {
                if (this != null && isActiveAndEnabled) target(evt);
            };
            return EventBus.Subscribe(safeHandler);
        }

        private void DisposeSubscriptions()
        {
            for (int i = subscriptions.Count - 1; i >= 0; i--) subscriptions[i].Dispose();
            subscriptions.Clear();
        }

        private static void RebindActiveSubscribers()
        {
            for (int i = activeSubscribers.Count - 1; i >= 0; i--)
            {
                EventSubscriberBehaviour subscriber = activeSubscribers[i];
                if (subscriber == null)
                {
                    activeSubscribers.RemoveAt(i);
                    continue;
                }
                subscriber.DisposeSubscriptions();
                if (subscriber.isActiveAndEnabled) subscriber.RegisterSubscriptions();
            }
        }

        private static HandlerDescriptor[] GetDescriptors(Type componentType)
        {
            HandlerDescriptor[] cached;
            if (descriptors.TryGetValue(componentType, out cached)) return cached;

            MethodInfo[] methods = componentType.GetMethods(BindingFlags.Instance | BindingFlags.Public |
                BindingFlags.NonPublic);
            List<HandlerDescriptor> found = new List<HandlerDescriptor>();
            for (int i = 0; i < methods.Length; i++)
            {
                if (!Attribute.IsDefined(methods[i], typeof(SubscribeEventAttribute), true)) continue;
                ParameterInfo[] parameters = methods[i].GetParameters();
                if (methods[i].IsStatic || methods[i].ReturnType != typeof(void) || parameters.Length != 1 ||
                    parameters[0].ParameterType.IsByRef)
                {
                    throw new InvalidOperationException("[SubscribeEvent] methods need one value parameter and a void return: " + methods[i]);
                }
                found.Add(new HandlerDescriptor { EventType = parameters[0].ParameterType, Method = methods[i] });
            }
            cached = found.ToArray();
            descriptors.Add(componentType, cached);
            return cached;
        }
    }
}
