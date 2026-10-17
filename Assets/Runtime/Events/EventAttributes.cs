using System;
using UnityMeta;

namespace YKW.Core.Events
{
    /// <summary>Marks a component method for lifecycle-managed event subscription.</summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class SubscribeEventAttribute : Attribute { }

    /// <summary>Publishes a typed value-change event after the field's final value changes.</summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class NotifyChangedAttribute : FieldChangeAspectAttribute
    {
        [ChangeTemplate]
        public static void Changed<TValue>(
            [TargetInstance] object source,
            [TargetMemberName] string memberName,
            [OldValue] TValue oldValue,
            [NewValue] TValue newValue)
        {
            ValueChangedPublisher.Publish(source, memberName, oldValue, newValue);
        }
    }

    /// <summary>
    /// Publishes a parameterless event after a method returns normally.
    /// Business success must still be checked explicitly by the method itself.
    /// </summary>
    public sealed class PublishEventAttribute : MethodAspectAttribute
    {
        public PublishEventAttribute(Type eventType) { }

        [AfterTemplate]
        public static void After([AspectArgument(0)] Type eventType,
            [TargetTypeName] string declaringType, [TargetMemberName] string methodName)
        {
            MethodEventPublisher.Publish(eventType, declaringType, methodName);
        }
    }

}
