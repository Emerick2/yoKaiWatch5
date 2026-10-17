using System;
using System.Collections.Generic;
using System.Reflection;

namespace YKW.Core.Events
{
    public interface IMethodEvent
    {
        void SetOrigin(string declaringType, string methodName);
    }

    public abstract class MethodEvent : IMethodEvent
    {
        public string DeclaringType { get; private set; }
        public string MethodName { get; private set; }
        public void SetOrigin(string declaringType, string methodName)
        {
            DeclaringType = declaringType;
            MethodName = methodName;
        }
    }

    public sealed class ValueChangedEvent<TEventValue>
    {
        public object Source { get; private set; }
        public string SourceType { get; private set; }
        public string MemberName { get; private set; }
        public TEventValue OldValue { get; private set; }
        public TEventValue NewValue { get; private set; }

        public ValueChangedEvent(string sourceType, string memberName, TEventValue oldValue, TEventValue newValue)
        {
            Source = null;
            SourceType = sourceType;
            MemberName = memberName;
            OldValue = oldValue;
            NewValue = newValue;
        }

        public ValueChangedEvent(object source, string memberName, TEventValue oldValue, TEventValue newValue)
        {
            Source = source;
            SourceType = source == null ? string.Empty : source.GetType().FullName;
            MemberName = memberName;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }

    /// <summary>A serializable-model-friendly observable value for non-Unity data.</summary>
    public sealed class ObservableValue<TValue>
    {
        private readonly object owner;
        private readonly string memberName;
        private TValue value;

        public ObservableValue(object owner, string memberName, TValue initialValue = default(TValue))
        {
            this.owner = owner;
            this.memberName = memberName ?? string.Empty;
            value = initialValue;
        }

        public TValue Value
        {
            get { return value; }
            set
            {
                if (EqualityComparer<TValue>.Default.Equals(this.value, value)) return;
                TValue previous = this.value;
                this.value = value;
                EventBus.Publish(new ValueChangedEvent<TValue>(owner, memberName, previous, value));
            }
        }
    }

    /// <summary>Runtime bridge used by UnityMeta-generated field templates.</summary>
    public static class ValueChangedPublisher
    {
        public static void Publish<TEventValue>(object source, string memberName,
            TEventValue oldValue, TEventValue newValue)
        {
            EventBus.PublishOnMainThread(new ValueChangedEvent<TEventValue>(source, memberName, oldValue, newValue));
        }

        public static void Publish<TEventValue>(string sourceType, string memberName,
            TEventValue oldValue, TEventValue newValue)
        {
            EventBus.PublishOnMainThread(new ValueChangedEvent<TEventValue>(sourceType, memberName, oldValue, newValue));
        }
    }

    /// <summary>Runtime bridge used by UnityMeta-generated method templates.</summary>
    public static class MethodEventPublisher
    {
        private static readonly Dictionary<Type, Func<object>> factories = new Dictionary<Type, Func<object>>();
        private static readonly object factoryLock = new object();
        private static readonly MethodInfo factoryMethod = typeof(MethodEventPublisher).GetMethod(
            nameof(CreateFactory), BindingFlags.NonPublic | BindingFlags.Static);

        public static void Publish(Type eventType, string declaringType, string methodName)
        {
            if (eventType == null) throw new ArgumentNullException(nameof(eventType));
            if (eventType.IsAbstract || eventType.IsInterface || eventType.ContainsGenericParameters)
                throw new InvalidOperationException("PublishEvent needs a concrete closed event type: " + eventType.FullName);

            Func<object> factory;
            lock (factoryLock)
            {
                if (!factories.TryGetValue(eventType, out factory))
                {
                    factory = (Func<object>)factoryMethod.MakeGenericMethod(eventType).Invoke(null, null);
                    factories.Add(eventType, factory);
                }
            }

            object eventData = factory();
            IMethodEvent methodEvent = eventData as IMethodEvent;
            if (methodEvent != null) methodEvent.SetOrigin(declaringType, methodName);
            EventBus.PublishObjectOnMainThread(eventType, eventData);
        }

        private static Func<object> CreateFactory<TEvent>() where TEvent : new()
        {
            return () => new TEvent();
        }
    }
}
