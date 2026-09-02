#nullable enable

using System;
using UnityMeta;


namespace YKW.Core.Commun
{
    /// <summary>
    /// Defines how Clamp handles NaN values for floating-point fields.
    /// </summary>
    public enum ClampNaN
    {
        Allow,
        Min,
        Max,
        Zero,
        Throw
    }

    /// <summary>
    /// Clamps writes to a numeric field between a minimum and a maximum.
    /// Bounds can be constants or sibling fields referenced with nameof(...).
    ///
    /// UnityMeta 0.2 note:
    /// - this aspect transforms normal C# field writes;
    /// - changing a dynamic bound does not yet automatically re-clamp dependants;
    /// - Unity Inspector serialization can bypass woven writes until Editor hooks land.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class ClampAttribute : FieldSetAspectAttribute
    {
        public ClampAttribute(double min, double max, ClampNaN nanBehavior = ClampNaN.Min) { }
        public ClampAttribute(double min, string maxMemberName, ClampNaN nanBehavior = ClampNaN.Min) { }
        public ClampAttribute(string minMemberName, double max, ClampNaN nanBehavior = ClampNaN.Min) { }
        public ClampAttribute(string minMemberName, string maxMemberName, ClampNaN nanBehavior = ClampNaN.Min) { }

        [SetTemplate]
        public static sbyte SByteConstant(
            [Value] sbyte value,
            [AspectArgument(0)] double min,
            [AspectArgument(1)] double max)
        {
            return ClampRuntime.Clamp(value, ClampRuntime.ToSByte(min, "min"), ClampRuntime.ToSByte(max, "max"));
        }

        [SetTemplate]
        public static sbyte SByteDynamicMax(
            [Value] sbyte value,
            [AspectArgument(0)] double min,
            [FieldValueFromAspectArgument(1)] sbyte max)
        {
            return ClampRuntime.Clamp(value, ClampRuntime.ToSByte(min, "min"), max);
        }

        [SetTemplate]
        public static sbyte SByteDynamicMin(
            [Value] sbyte value,
            [FieldValueFromAspectArgument(0)] sbyte min,
            [AspectArgument(1)] double max)
        {
            return ClampRuntime.Clamp(value, min, ClampRuntime.ToSByte(max, "max"));
        }

        [SetTemplate]
        public static sbyte SByteDynamicBoth(
            [Value] sbyte value,
            [FieldValueFromAspectArgument(0)] sbyte min,
            [FieldValueFromAspectArgument(1)] sbyte max)
        {
            return ClampRuntime.Clamp(value, min, max);
        }

        [SetTemplate]
        public static byte ByteConstant(
            [Value] byte value,
            [AspectArgument(0)] double min,
            [AspectArgument(1)] double max)
        {
            return ClampRuntime.Clamp(value, ClampRuntime.ToByte(min, "min"), ClampRuntime.ToByte(max, "max"));
        }

        [SetTemplate]
        public static byte ByteDynamicMax(
            [Value] byte value,
            [AspectArgument(0)] double min,
            [FieldValueFromAspectArgument(1)] byte max)
        {
            return ClampRuntime.Clamp(value, ClampRuntime.ToByte(min, "min"), max);
        }

        [SetTemplate]
        public static byte ByteDynamicMin(
            [Value] byte value,
            [FieldValueFromAspectArgument(0)] byte min,
            [AspectArgument(1)] double max)
        {
            return ClampRuntime.Clamp(value, min, ClampRuntime.ToByte(max, "max"));
        }

        [SetTemplate]
        public static byte ByteDynamicBoth(
            [Value] byte value,
            [FieldValueFromAspectArgument(0)] byte min,
            [FieldValueFromAspectArgument(1)] byte max)
        {
            return ClampRuntime.Clamp(value, min, max);
        }

        [SetTemplate]
        public static short Int16Constant(
            [Value] short value,
            [AspectArgument(0)] double min,
            [AspectArgument(1)] double max)
        {
            return ClampRuntime.Clamp(value, ClampRuntime.ToInt16(min, "min"), ClampRuntime.ToInt16(max, "max"));
        }

        [SetTemplate]
        public static short Int16DynamicMax(
            [Value] short value,
            [AspectArgument(0)] double min,
            [FieldValueFromAspectArgument(1)] short max)
        {
            return ClampRuntime.Clamp(value, ClampRuntime.ToInt16(min, "min"), max);
        }

        [SetTemplate]
        public static short Int16DynamicMin(
            [Value] short value,
            [FieldValueFromAspectArgument(0)] short min,
            [AspectArgument(1)] double max)
        {
            return ClampRuntime.Clamp(value, min, ClampRuntime.ToInt16(max, "max"));
        }

        [SetTemplate]
        public static short Int16DynamicBoth(
            [Value] short value,
            [FieldValueFromAspectArgument(0)] short min,
            [FieldValueFromAspectArgument(1)] short max)
        {
            return ClampRuntime.Clamp(value, min, max);
        }

        [SetTemplate]
        public static ushort UInt16Constant(
            [Value] ushort value,
            [AspectArgument(0)] double min,
            [AspectArgument(1)] double max)
        {
            return ClampRuntime.Clamp(value, ClampRuntime.ToUInt16(min, "min"), ClampRuntime.ToUInt16(max, "max"));
        }

        [SetTemplate]
        public static ushort UInt16DynamicMax(
            [Value] ushort value,
            [AspectArgument(0)] double min,
            [FieldValueFromAspectArgument(1)] ushort max)
        {
            return ClampRuntime.Clamp(value, ClampRuntime.ToUInt16(min, "min"), max);
        }

        [SetTemplate]
        public static ushort UInt16DynamicMin(
            [Value] ushort value,
            [FieldValueFromAspectArgument(0)] ushort min,
            [AspectArgument(1)] double max)
        {
            return ClampRuntime.Clamp(value, min, ClampRuntime.ToUInt16(max, "max"));
        }

        [SetTemplate]
        public static ushort UInt16DynamicBoth(
            [Value] ushort value,
            [FieldValueFromAspectArgument(0)] ushort min,
            [FieldValueFromAspectArgument(1)] ushort max)
        {
            return ClampRuntime.Clamp(value, min, max);
        }

        [SetTemplate]
        public static int Int32Constant(
            [Value] int value,
            [AspectArgument(0)] double min,
            [AspectArgument(1)] double max)
        {
            return ClampRuntime.Clamp(value, ClampRuntime.ToInt32(min, "min"), ClampRuntime.ToInt32(max, "max"));
        }

        [SetTemplate]
        public static int Int32DynamicMax(
            [Value] int value,
            [AspectArgument(0)] double min,
            [FieldValueFromAspectArgument(1)] int max)
        {
            return ClampRuntime.Clamp(value, ClampRuntime.ToInt32(min, "min"), max);
        }

        [SetTemplate]
        public static int Int32DynamicMin(
            [Value] int value,
            [FieldValueFromAspectArgument(0)] int min,
            [AspectArgument(1)] double max)
        {
            return ClampRuntime.Clamp(value, min, ClampRuntime.ToInt32(max, "max"));
        }

        [SetTemplate]
        public static int Int32DynamicBoth(
            [Value] int value,
            [FieldValueFromAspectArgument(0)] int min,
            [FieldValueFromAspectArgument(1)] int max)
        {
            return ClampRuntime.Clamp(value, min, max);
        }

        [SetTemplate]
        public static uint UInt32Constant(
            [Value] uint value,
            [AspectArgument(0)] double min,
            [AspectArgument(1)] double max)
        {
            return ClampRuntime.Clamp(value, ClampRuntime.ToUInt32(min, "min"), ClampRuntime.ToUInt32(max, "max"));
        }

        [SetTemplate]
        public static uint UInt32DynamicMax(
            [Value] uint value,
            [AspectArgument(0)] double min,
            [FieldValueFromAspectArgument(1)] uint max)
        {
            return ClampRuntime.Clamp(value, ClampRuntime.ToUInt32(min, "min"), max);
        }

        [SetTemplate]
        public static uint UInt32DynamicMin(
            [Value] uint value,
            [FieldValueFromAspectArgument(0)] uint min,
            [AspectArgument(1)] double max)
        {
            return ClampRuntime.Clamp(value, min, ClampRuntime.ToUInt32(max, "max"));
        }

        [SetTemplate]
        public static uint UInt32DynamicBoth(
            [Value] uint value,
            [FieldValueFromAspectArgument(0)] uint min,
            [FieldValueFromAspectArgument(1)] uint max)
        {
            return ClampRuntime.Clamp(value, min, max);
        }

        [SetTemplate]
        public static long Int64Constant(
            [Value] long value,
            [AspectArgument(0)] double min,
            [AspectArgument(1)] double max)
        {
            return ClampRuntime.Clamp(value, ClampRuntime.ToInt64(min, "min"), ClampRuntime.ToInt64(max, "max"));
        }

        [SetTemplate]
        public static long Int64DynamicMax(
            [Value] long value,
            [AspectArgument(0)] double min,
            [FieldValueFromAspectArgument(1)] long max)
        {
            return ClampRuntime.Clamp(value, ClampRuntime.ToInt64(min, "min"), max);
        }

        [SetTemplate]
        public static long Int64DynamicMin(
            [Value] long value,
            [FieldValueFromAspectArgument(0)] long min,
            [AspectArgument(1)] double max)
        {
            return ClampRuntime.Clamp(value, min, ClampRuntime.ToInt64(max, "max"));
        }

        [SetTemplate]
        public static long Int64DynamicBoth(
            [Value] long value,
            [FieldValueFromAspectArgument(0)] long min,
            [FieldValueFromAspectArgument(1)] long max)
        {
            return ClampRuntime.Clamp(value, min, max);
        }

        [SetTemplate]
        public static ulong UInt64Constant(
            [Value] ulong value,
            [AspectArgument(0)] double min,
            [AspectArgument(1)] double max)
        {
            return ClampRuntime.Clamp(value, ClampRuntime.ToUInt64(min, "min"), ClampRuntime.ToUInt64(max, "max"));
        }

        [SetTemplate]
        public static ulong UInt64DynamicMax(
            [Value] ulong value,
            [AspectArgument(0)] double min,
            [FieldValueFromAspectArgument(1)] ulong max)
        {
            return ClampRuntime.Clamp(value, ClampRuntime.ToUInt64(min, "min"), max);
        }

        [SetTemplate]
        public static ulong UInt64DynamicMin(
            [Value] ulong value,
            [FieldValueFromAspectArgument(0)] ulong min,
            [AspectArgument(1)] double max)
        {
            return ClampRuntime.Clamp(value, min, ClampRuntime.ToUInt64(max, "max"));
        }

        [SetTemplate]
        public static ulong UInt64DynamicBoth(
            [Value] ulong value,
            [FieldValueFromAspectArgument(0)] ulong min,
            [FieldValueFromAspectArgument(1)] ulong max)
        {
            return ClampRuntime.Clamp(value, min, max);
        }

        [SetTemplate]
        public static float SingleConstant(
            [Value] float value,
            [AspectArgument(0)] double min,
            [AspectArgument(1)] double max,
            [AspectArgument(2)] ClampNaN nanBehavior)
        {
            return ClampRuntime.Clamp(value, ClampRuntime.ToSingle(min, "min"), ClampRuntime.ToSingle(max, "max"), nanBehavior);
        }

        [SetTemplate]
        public static float SingleDynamicMax(
            [Value] float value,
            [AspectArgument(0)] double min,
            [FieldValueFromAspectArgument(1)] float max,
            [AspectArgument(2)] ClampNaN nanBehavior)
        {
            return ClampRuntime.Clamp(value, ClampRuntime.ToSingle(min, "min"), max, nanBehavior);
        }

        [SetTemplate]
        public static float SingleDynamicMin(
            [Value] float value,
            [FieldValueFromAspectArgument(0)] float min,
            [AspectArgument(1)] double max,
            [AspectArgument(2)] ClampNaN nanBehavior)
        {
            return ClampRuntime.Clamp(value, min, ClampRuntime.ToSingle(max, "max"), nanBehavior);
        }

        [SetTemplate]
        public static float SingleDynamicBoth(
            [Value] float value,
            [FieldValueFromAspectArgument(0)] float min,
            [FieldValueFromAspectArgument(1)] float max,
            [AspectArgument(2)] ClampNaN nanBehavior)
        {
            return ClampRuntime.Clamp(value, min, max, nanBehavior);
        }

        [SetTemplate]
        public static double DoubleConstant(
            [Value] double value,
            [AspectArgument(0)] double min,
            [AspectArgument(1)] double max,
            [AspectArgument(2)] ClampNaN nanBehavior)
        {
            return ClampRuntime.Clamp(value, ClampRuntime.ToDouble(min, "min"), ClampRuntime.ToDouble(max, "max"), nanBehavior);
        }

        [SetTemplate]
        public static double DoubleDynamicMax(
            [Value] double value,
            [AspectArgument(0)] double min,
            [FieldValueFromAspectArgument(1)] double max,
            [AspectArgument(2)] ClampNaN nanBehavior)
        {
            return ClampRuntime.Clamp(value, ClampRuntime.ToDouble(min, "min"), max, nanBehavior);
        }

        [SetTemplate]
        public static double DoubleDynamicMin(
            [Value] double value,
            [FieldValueFromAspectArgument(0)] double min,
            [AspectArgument(1)] double max,
            [AspectArgument(2)] ClampNaN nanBehavior)
        {
            return ClampRuntime.Clamp(value, min, ClampRuntime.ToDouble(max, "max"), nanBehavior);
        }

        [SetTemplate]
        public static double DoubleDynamicBoth(
            [Value] double value,
            [FieldValueFromAspectArgument(0)] double min,
            [FieldValueFromAspectArgument(1)] double max,
            [AspectArgument(2)] ClampNaN nanBehavior)
        {
            return ClampRuntime.Clamp(value, min, max, nanBehavior);
        }

        [SetTemplate]
        public static decimal DecimalConstant(
            [Value] decimal value,
            [AspectArgument(0)] double min,
            [AspectArgument(1)] double max)
        {
            return ClampRuntime.Clamp(value, ClampRuntime.ToDecimal(min, "min"), ClampRuntime.ToDecimal(max, "max"));
        }

        [SetTemplate]
        public static decimal DecimalDynamicMax(
            [Value] decimal value,
            [AspectArgument(0)] double min,
            [FieldValueFromAspectArgument(1)] decimal max)
        {
            return ClampRuntime.Clamp(value, ClampRuntime.ToDecimal(min, "min"), max);
        }

        [SetTemplate]
        public static decimal DecimalDynamicMin(
            [Value] decimal value,
            [FieldValueFromAspectArgument(0)] decimal min,
            [AspectArgument(1)] double max)
        {
            return ClampRuntime.Clamp(value, min, ClampRuntime.ToDecimal(max, "max"));
        }

        [SetTemplate]
        public static decimal DecimalDynamicBoth(
            [Value] decimal value,
            [FieldValueFromAspectArgument(0)] decimal min,
            [FieldValueFromAspectArgument(1)] decimal max)
        {
            return ClampRuntime.Clamp(value, min, max);
        }

    }

    /// <summary>
    /// Clamps writes to a numeric field to a minimum value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class ClampMinAttribute : FieldSetAspectAttribute
    {
        public ClampMinAttribute(double min, ClampNaN nanBehavior = ClampNaN.Min) { }
        public ClampMinAttribute(string minMemberName, ClampNaN nanBehavior = ClampNaN.Min) { }

        [SetTemplate]
        public static sbyte SByteConstant(
            [Value] sbyte value,
            [AspectArgument(0)] double min)
        {
            return ClampRuntime.ClampMin(value, ClampRuntime.ToSByte(min, "min"));
        }

        [SetTemplate]
        public static sbyte SByteDynamic(
            [Value] sbyte value,
            [FieldValueFromAspectArgument(0)] sbyte min)
        {
            return ClampRuntime.ClampMin(value, min);
        }

        [SetTemplate]
        public static byte ByteConstant(
            [Value] byte value,
            [AspectArgument(0)] double min)
        {
            return ClampRuntime.ClampMin(value, ClampRuntime.ToByte(min, "min"));
        }

        [SetTemplate]
        public static byte ByteDynamic(
            [Value] byte value,
            [FieldValueFromAspectArgument(0)] byte min)
        {
            return ClampRuntime.ClampMin(value, min);
        }

        [SetTemplate]
        public static short Int16Constant(
            [Value] short value,
            [AspectArgument(0)] double min)
        {
            return ClampRuntime.ClampMin(value, ClampRuntime.ToInt16(min, "min"));
        }

        [SetTemplate]
        public static short Int16Dynamic(
            [Value] short value,
            [FieldValueFromAspectArgument(0)] short min)
        {
            return ClampRuntime.ClampMin(value, min);
        }

        [SetTemplate]
        public static ushort UInt16Constant(
            [Value] ushort value,
            [AspectArgument(0)] double min)
        {
            return ClampRuntime.ClampMin(value, ClampRuntime.ToUInt16(min, "min"));
        }

        [SetTemplate]
        public static ushort UInt16Dynamic(
            [Value] ushort value,
            [FieldValueFromAspectArgument(0)] ushort min)
        {
            return ClampRuntime.ClampMin(value, min);
        }

        [SetTemplate]
        public static int Int32Constant(
            [Value] int value,
            [AspectArgument(0)] double min)
        {
            return ClampRuntime.ClampMin(value, ClampRuntime.ToInt32(min, "min"));
        }

        [SetTemplate]
        public static int Int32Dynamic(
            [Value] int value,
            [FieldValueFromAspectArgument(0)] int min)
        {
            return ClampRuntime.ClampMin(value, min);
        }

        [SetTemplate]
        public static uint UInt32Constant(
            [Value] uint value,
            [AspectArgument(0)] double min)
        {
            return ClampRuntime.ClampMin(value, ClampRuntime.ToUInt32(min, "min"));
        }

        [SetTemplate]
        public static uint UInt32Dynamic(
            [Value] uint value,
            [FieldValueFromAspectArgument(0)] uint min)
        {
            return ClampRuntime.ClampMin(value, min);
        }

        [SetTemplate]
        public static long Int64Constant(
            [Value] long value,
            [AspectArgument(0)] double min)
        {
            return ClampRuntime.ClampMin(value, ClampRuntime.ToInt64(min, "min"));
        }

        [SetTemplate]
        public static long Int64Dynamic(
            [Value] long value,
            [FieldValueFromAspectArgument(0)] long min)
        {
            return ClampRuntime.ClampMin(value, min);
        }

        [SetTemplate]
        public static ulong UInt64Constant(
            [Value] ulong value,
            [AspectArgument(0)] double min)
        {
            return ClampRuntime.ClampMin(value, ClampRuntime.ToUInt64(min, "min"));
        }

        [SetTemplate]
        public static ulong UInt64Dynamic(
            [Value] ulong value,
            [FieldValueFromAspectArgument(0)] ulong min)
        {
            return ClampRuntime.ClampMin(value, min);
        }

        [SetTemplate]
        public static float SingleConstant(
            [Value] float value,
            [AspectArgument(0)] double min,
            [AspectArgument(1)] ClampNaN nanBehavior)
        {
            return ClampRuntime.ClampMin(value, ClampRuntime.ToSingle(min, "min"), nanBehavior);
        }

        [SetTemplate]
        public static float SingleDynamic(
            [Value] float value,
            [FieldValueFromAspectArgument(0)] float min,
            [AspectArgument(1)] ClampNaN nanBehavior)
        {
            return ClampRuntime.ClampMin(value, min, nanBehavior);
        }

        [SetTemplate]
        public static double DoubleConstant(
            [Value] double value,
            [AspectArgument(0)] double min,
            [AspectArgument(1)] ClampNaN nanBehavior)
        {
            return ClampRuntime.ClampMin(value, ClampRuntime.ToDouble(min, "min"), nanBehavior);
        }

        [SetTemplate]
        public static double DoubleDynamic(
            [Value] double value,
            [FieldValueFromAspectArgument(0)] double min,
            [AspectArgument(1)] ClampNaN nanBehavior)
        {
            return ClampRuntime.ClampMin(value, min, nanBehavior);
        }

        [SetTemplate]
        public static decimal DecimalConstant(
            [Value] decimal value,
            [AspectArgument(0)] double min)
        {
            return ClampRuntime.ClampMin(value, ClampRuntime.ToDecimal(min, "min"));
        }

        [SetTemplate]
        public static decimal DecimalDynamic(
            [Value] decimal value,
            [FieldValueFromAspectArgument(0)] decimal min)
        {
            return ClampRuntime.ClampMin(value, min);
        }

    }

    /// <summary>
    /// Clamps writes to a numeric field to a maximum value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class ClampMaxAttribute : FieldSetAspectAttribute
    {
        public ClampMaxAttribute(double max, ClampNaN nanBehavior = ClampNaN.Max) { }
        public ClampMaxAttribute(string maxMemberName, ClampNaN nanBehavior = ClampNaN.Max) { }

        [SetTemplate]
        public static sbyte SByteConstant(
            [Value] sbyte value,
            [AspectArgument(0)] double max)
        {
            return ClampRuntime.ClampMax(value, ClampRuntime.ToSByte(max, "max"));
        }

        [SetTemplate]
        public static sbyte SByteDynamic(
            [Value] sbyte value,
            [FieldValueFromAspectArgument(0)] sbyte max)
        {
            return ClampRuntime.ClampMax(value, max);
        }

        [SetTemplate]
        public static byte ByteConstant(
            [Value] byte value,
            [AspectArgument(0)] double max)
        {
            return ClampRuntime.ClampMax(value, ClampRuntime.ToByte(max, "max"));
        }

        [SetTemplate]
        public static byte ByteDynamic(
            [Value] byte value,
            [FieldValueFromAspectArgument(0)] byte max)
        {
            return ClampRuntime.ClampMax(value, max);
        }

        [SetTemplate]
        public static short Int16Constant(
            [Value] short value,
            [AspectArgument(0)] double max)
        {
            return ClampRuntime.ClampMax(value, ClampRuntime.ToInt16(max, "max"));
        }

        [SetTemplate]
        public static short Int16Dynamic(
            [Value] short value,
            [FieldValueFromAspectArgument(0)] short max)
        {
            return ClampRuntime.ClampMax(value, max);
        }

        [SetTemplate]
        public static ushort UInt16Constant(
            [Value] ushort value,
            [AspectArgument(0)] double max)
        {
            return ClampRuntime.ClampMax(value, ClampRuntime.ToUInt16(max, "max"));
        }

        [SetTemplate]
        public static ushort UInt16Dynamic(
            [Value] ushort value,
            [FieldValueFromAspectArgument(0)] ushort max)
        {
            return ClampRuntime.ClampMax(value, max);
        }

        [SetTemplate]
        public static int Int32Constant(
            [Value] int value,
            [AspectArgument(0)] double max)
        {
            return ClampRuntime.ClampMax(value, ClampRuntime.ToInt32(max, "max"));
        }

        [SetTemplate]
        public static int Int32Dynamic(
            [Value] int value,
            [FieldValueFromAspectArgument(0)] int max)
        {
            return ClampRuntime.ClampMax(value, max);
        }

        [SetTemplate]
        public static uint UInt32Constant(
            [Value] uint value,
            [AspectArgument(0)] double max)
        {
            return ClampRuntime.ClampMax(value, ClampRuntime.ToUInt32(max, "max"));
        }

        [SetTemplate]
        public static uint UInt32Dynamic(
            [Value] uint value,
            [FieldValueFromAspectArgument(0)] uint max)
        {
            return ClampRuntime.ClampMax(value, max);
        }

        [SetTemplate]
        public static long Int64Constant(
            [Value] long value,
            [AspectArgument(0)] double max)
        {
            return ClampRuntime.ClampMax(value, ClampRuntime.ToInt64(max, "max"));
        }

        [SetTemplate]
        public static long Int64Dynamic(
            [Value] long value,
            [FieldValueFromAspectArgument(0)] long max)
        {
            return ClampRuntime.ClampMax(value, max);
        }

        [SetTemplate]
        public static ulong UInt64Constant(
            [Value] ulong value,
            [AspectArgument(0)] double max)
        {
            return ClampRuntime.ClampMax(value, ClampRuntime.ToUInt64(max, "max"));
        }

        [SetTemplate]
        public static ulong UInt64Dynamic(
            [Value] ulong value,
            [FieldValueFromAspectArgument(0)] ulong max)
        {
            return ClampRuntime.ClampMax(value, max);
        }

        [SetTemplate]
        public static float SingleConstant(
            [Value] float value,
            [AspectArgument(0)] double max,
            [AspectArgument(1)] ClampNaN nanBehavior)
        {
            return ClampRuntime.ClampMax(value, ClampRuntime.ToSingle(max, "max"), nanBehavior);
        }

        [SetTemplate]
        public static float SingleDynamic(
            [Value] float value,
            [FieldValueFromAspectArgument(0)] float max,
            [AspectArgument(1)] ClampNaN nanBehavior)
        {
            return ClampRuntime.ClampMax(value, max, nanBehavior);
        }

        [SetTemplate]
        public static double DoubleConstant(
            [Value] double value,
            [AspectArgument(0)] double max,
            [AspectArgument(1)] ClampNaN nanBehavior)
        {
            return ClampRuntime.ClampMax(value, ClampRuntime.ToDouble(max, "max"), nanBehavior);
        }

        [SetTemplate]
        public static double DoubleDynamic(
            [Value] double value,
            [FieldValueFromAspectArgument(0)] double max,
            [AspectArgument(1)] ClampNaN nanBehavior)
        {
            return ClampRuntime.ClampMax(value, max, nanBehavior);
        }

        [SetTemplate]
        public static decimal DecimalConstant(
            [Value] decimal value,
            [AspectArgument(0)] double max)
        {
            return ClampRuntime.ClampMax(value, ClampRuntime.ToDecimal(max, "max"));
        }

        [SetTemplate]
        public static decimal DecimalDynamic(
            [Value] decimal value,
            [FieldValueFromAspectArgument(0)] decimal max)
        {
            return ClampRuntime.ClampMax(value, max);
        }

    }

    /// <summary>
    /// Runtime helpers used by the woven template calls.
    /// Kept separate from the aspect metadata so the attribute itself stays declarative.
    /// </summary>
    internal static class ClampRuntime
    {
        internal static sbyte ToSByte(double value, string name)
        {
            if (double.IsNaN(value) || double.IsInfinity(value) ||
                Math.Truncate(value) != value || value < sbyte.MinValue || value > sbyte.MaxValue)
            {
                throw new ArgumentOutOfRangeException(name, value, "The bound cannot be represented by sbyte.");
            }

            return (sbyte)value;
        }

        internal static byte ToByte(double value, string name)
        {
            if (double.IsNaN(value) || double.IsInfinity(value) ||
                Math.Truncate(value) != value || value < byte.MinValue || value > byte.MaxValue)
            {
                throw new ArgumentOutOfRangeException(name, value, "The bound cannot be represented by byte.");
            }

            return (byte)value;
        }

        internal static short ToInt16(double value, string name)
        {
            if (double.IsNaN(value) || double.IsInfinity(value) ||
                Math.Truncate(value) != value || value < short.MinValue || value > short.MaxValue)
            {
                throw new ArgumentOutOfRangeException(name, value, "The bound cannot be represented by short.");
            }

            return (short)value;
        }

        internal static ushort ToUInt16(double value, string name)
        {
            if (double.IsNaN(value) || double.IsInfinity(value) ||
                Math.Truncate(value) != value || value < ushort.MinValue || value > ushort.MaxValue)
            {
                throw new ArgumentOutOfRangeException(name, value, "The bound cannot be represented by ushort.");
            }

            return (ushort)value;
        }

        internal static int ToInt32(double value, string name)
        {
            if (double.IsNaN(value) || double.IsInfinity(value) ||
                Math.Truncate(value) != value || value < int.MinValue || value > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException(name, value, "The bound cannot be represented by int.");
            }

            return (int)value;
        }

        internal static uint ToUInt32(double value, string name)
        {
            if (double.IsNaN(value) || double.IsInfinity(value) ||
                Math.Truncate(value) != value || value < uint.MinValue || value > uint.MaxValue)
            {
                throw new ArgumentOutOfRangeException(name, value, "The bound cannot be represented by uint.");
            }

            return (uint)value;
        }

        internal static long ToInt64(double value, string name)
        {
            if (double.IsNaN(value) || double.IsInfinity(value) ||
                Math.Truncate(value) != value || value < -9007199254740992d || value > 9007199254740992d)
            {
                throw new ArgumentOutOfRangeException(name, value, "Constants targeting Int64 are limited to +/-2^53 because Clamp metadata uses double constants.");
            }

            return (long)value;
        }

        internal static ulong ToUInt64(double value, string name)
        {
            if (double.IsNaN(value) || double.IsInfinity(value) ||
                Math.Truncate(value) != value || value < 0d || value > 9007199254740992d)
            {
                throw new ArgumentOutOfRangeException(name, value, "Constants targeting UInt64 are limited to 2^53 because Clamp metadata uses double constants.");
            }

            return (ulong)value;
        }

        internal static float ToSingle(double value, string name)
        {
            if (double.IsNaN(value) ||
                (!double.IsInfinity(value) && (value < -float.MaxValue || value > float.MaxValue)))
            {
                throw new ArgumentOutOfRangeException(name, value, "The bound cannot be represented by float.");
            }

            return (float)value;
        }

        internal static double ToDouble(double value, string name)
        {
            if (double.IsNaN(value))
            {
                throw new ArgumentOutOfRangeException(name, value, "A Clamp bound cannot be NaN.");
            }

            return value;
        }

        internal static decimal ToDecimal(double value, string name)
        {
            if (double.IsNaN(value) || double.IsInfinity(value) ||
                value < (double)decimal.MinValue || value > (double)decimal.MaxValue)
            {
                throw new ArgumentOutOfRangeException(name, value, "The bound cannot be represented by decimal.");
            }

            return (decimal)value;
        }

        public static sbyte Clamp(sbyte value, sbyte min, sbyte max)
        {
            ValidateOrder(min, max);
            return value < min ? min : value > max ? max : value;
        }

        public static sbyte ClampMin(sbyte value, sbyte min)
        {
            return value < min ? min : value;
        }

        public static sbyte ClampMax(sbyte value, sbyte max)
        {
            return value > max ? max : value;
        }

        public static byte Clamp(byte value, byte min, byte max)
        {
            ValidateOrder(min, max);
            return value < min ? min : value > max ? max : value;
        }

        public static byte ClampMin(byte value, byte min)
        {
            return value < min ? min : value;
        }

        public static byte ClampMax(byte value, byte max)
        {
            return value > max ? max : value;
        }

        public static short Clamp(short value, short min, short max)
        {
            ValidateOrder(min, max);
            return value < min ? min : value > max ? max : value;
        }

        public static short ClampMin(short value, short min)
        {
            return value < min ? min : value;
        }

        public static short ClampMax(short value, short max)
        {
            return value > max ? max : value;
        }

        public static ushort Clamp(ushort value, ushort min, ushort max)
        {
            ValidateOrder(min, max);
            return value < min ? min : value > max ? max : value;
        }

        public static ushort ClampMin(ushort value, ushort min)
        {
            return value < min ? min : value;
        }

        public static ushort ClampMax(ushort value, ushort max)
        {
            return value > max ? max : value;
        }

        public static int Clamp(int value, int min, int max)
        {
            ValidateOrder(min, max);
            return value < min ? min : value > max ? max : value;
        }

        public static int ClampMin(int value, int min)
        {
            return value < min ? min : value;
        }

        public static int ClampMax(int value, int max)
        {
            return value > max ? max : value;
        }

        public static uint Clamp(uint value, uint min, uint max)
        {
            ValidateOrder(min, max);
            return value < min ? min : value > max ? max : value;
        }

        public static uint ClampMin(uint value, uint min)
        {
            return value < min ? min : value;
        }

        public static uint ClampMax(uint value, uint max)
        {
            return value > max ? max : value;
        }

        public static long Clamp(long value, long min, long max)
        {
            ValidateOrder(min, max);
            return value < min ? min : value > max ? max : value;
        }

        public static long ClampMin(long value, long min)
        {
            return value < min ? min : value;
        }

        public static long ClampMax(long value, long max)
        {
            return value > max ? max : value;
        }

        public static ulong Clamp(ulong value, ulong min, ulong max)
        {
            ValidateOrder(min, max);
            return value < min ? min : value > max ? max : value;
        }

        public static ulong ClampMin(ulong value, ulong min)
        {
            return value < min ? min : value;
        }

        public static ulong ClampMax(ulong value, ulong max)
        {
            return value > max ? max : value;
        }

        public static decimal Clamp(decimal value, decimal min, decimal max)
        {
            ValidateOrder(min, max);
            return value < min ? min : value > max ? max : value;
        }

        public static decimal ClampMin(decimal value, decimal min)
        {
            return value < min ? min : value;
        }

        public static decimal ClampMax(decimal value, decimal max)
        {
            return value > max ? max : value;
        }

        public static float Clamp(float value, float min, float max, ClampNaN nanBehavior)
        {
            ValidateFloatingBounds(min, max);
            if (float.IsNaN(value))
            {
                switch (nanBehavior)
                {
                    case ClampNaN.Allow:
                        return value;
                    case ClampNaN.Min:
                        return min;
                    case ClampNaN.Max:
                        return max;
                    case ClampNaN.Zero:
                        value = 0f;
                        break;
                    case ClampNaN.Throw:
                        throw new ArgumentOutOfRangeException("value", "NaN is not allowed for this clamped field.");
                    default:
                        throw new ArgumentOutOfRangeException("nanBehavior");
                }
            }

            return value < min ? min : value > max ? max : value;
        }

        public static float ClampMin(float value, float min, ClampNaN nanBehavior)
        {
            if (float.IsNaN(min))
            {
                throw new InvalidOperationException("A Clamp minimum cannot be NaN.");
            }

            if (float.IsNaN(value))
            {
                switch (nanBehavior)
                {
                    case ClampNaN.Allow:
                        return value;
                    case ClampNaN.Min:
                        return min;
                    case ClampNaN.Zero:
                        value = 0f;
                        break;
                    case ClampNaN.Throw:
                        throw new ArgumentOutOfRangeException("value", "NaN is not allowed for this clamped field.");
                    case ClampNaN.Max:
                        throw new InvalidOperationException("ClampNaN.Max requires a maximum bound.");
                    default:
                        throw new ArgumentOutOfRangeException("nanBehavior");
                }
            }

            return value < min ? min : value;
        }

        public static float ClampMax(float value, float max, ClampNaN nanBehavior)
        {
            if (float.IsNaN(max))
            {
                throw new InvalidOperationException("A Clamp maximum cannot be NaN.");
            }

            if (float.IsNaN(value))
            {
                switch (nanBehavior)
                {
                    case ClampNaN.Allow:
                        return value;
                    case ClampNaN.Max:
                        return max;
                    case ClampNaN.Zero:
                        value = 0f;
                        break;
                    case ClampNaN.Throw:
                        throw new ArgumentOutOfRangeException("value", "NaN is not allowed for this clamped field.");
                    case ClampNaN.Min:
                        throw new InvalidOperationException("ClampNaN.Min requires a minimum bound.");
                    default:
                        throw new ArgumentOutOfRangeException("nanBehavior");
                }
            }

            return value > max ? max : value;
        }

        public static double Clamp(double value, double min, double max, ClampNaN nanBehavior)
        {
            ValidateFloatingBounds(min, max);
            if (double.IsNaN(value))
            {
                switch (nanBehavior)
                {
                    case ClampNaN.Allow:
                        return value;
                    case ClampNaN.Min:
                        return min;
                    case ClampNaN.Max:
                        return max;
                    case ClampNaN.Zero:
                        value = 0d;
                        break;
                    case ClampNaN.Throw:
                        throw new ArgumentOutOfRangeException("value", "NaN is not allowed for this clamped field.");
                    default:
                        throw new ArgumentOutOfRangeException("nanBehavior");
                }
            }

            return value < min ? min : value > max ? max : value;
        }

        public static double ClampMin(double value, double min, ClampNaN nanBehavior)
        {
            if (double.IsNaN(min))
            {
                throw new InvalidOperationException("A Clamp minimum cannot be NaN.");
            }

            if (double.IsNaN(value))
            {
                switch (nanBehavior)
                {
                    case ClampNaN.Allow:
                        return value;
                    case ClampNaN.Min:
                        return min;
                    case ClampNaN.Zero:
                        value = 0d;
                        break;
                    case ClampNaN.Throw:
                        throw new ArgumentOutOfRangeException("value", "NaN is not allowed for this clamped field.");
                    case ClampNaN.Max:
                        throw new InvalidOperationException("ClampNaN.Max requires a maximum bound.");
                    default:
                        throw new ArgumentOutOfRangeException("nanBehavior");
                }
            }

            return value < min ? min : value;
        }

        public static double ClampMax(double value, double max, ClampNaN nanBehavior)
        {
            if (double.IsNaN(max))
            {
                throw new InvalidOperationException("A Clamp maximum cannot be NaN.");
            }

            if (double.IsNaN(value))
            {
                switch (nanBehavior)
                {
                    case ClampNaN.Allow:
                        return value;
                    case ClampNaN.Max:
                        return max;
                    case ClampNaN.Zero:
                        value = 0d;
                        break;
                    case ClampNaN.Throw:
                        throw new ArgumentOutOfRangeException("value", "NaN is not allowed for this clamped field.");
                    case ClampNaN.Min:
                        throw new InvalidOperationException("ClampNaN.Min requires a minimum bound.");
                    default:
                        throw new ArgumentOutOfRangeException("nanBehavior");
                }
            }

            return value > max ? max : value;
        }

        private static void ValidateOrder(sbyte min, sbyte max)
        {
            if (min > max)
            {
                throw new InvalidOperationException("Clamp minimum is greater than maximum.");
            }
        }

        private static void ValidateOrder(byte min, byte max)
        {
            if (min > max)
            {
                throw new InvalidOperationException("Clamp minimum is greater than maximum.");
            }
        }

        private static void ValidateOrder(short min, short max)
        {
            if (min > max)
            {
                throw new InvalidOperationException("Clamp minimum is greater than maximum.");
            }
        }

        private static void ValidateOrder(ushort min, ushort max)
        {
            if (min > max)
            {
                throw new InvalidOperationException("Clamp minimum is greater than maximum.");
            }
        }

        private static void ValidateOrder(int min, int max)
        {
            if (min > max)
            {
                throw new InvalidOperationException("Clamp minimum is greater than maximum.");
            }
        }

        private static void ValidateOrder(uint min, uint max)
        {
            if (min > max)
            {
                throw new InvalidOperationException("Clamp minimum is greater than maximum.");
            }
        }

        private static void ValidateOrder(long min, long max)
        {
            if (min > max)
            {
                throw new InvalidOperationException("Clamp minimum is greater than maximum.");
            }
        }

        private static void ValidateOrder(ulong min, ulong max)
        {
            if (min > max)
            {
                throw new InvalidOperationException("Clamp minimum is greater than maximum.");
            }
        }

        private static void ValidateOrder(decimal min, decimal max)
        {
            if (min > max)
            {
                throw new InvalidOperationException("Clamp minimum is greater than maximum.");
            }
        }

        private static void ValidateFloatingBounds(float min, float max)
        {
            if (float.IsNaN(min) || float.IsNaN(max))
            {
                throw new InvalidOperationException("Clamp bounds cannot be NaN.");
            }

            if (min > max)
            {
                throw new InvalidOperationException("Clamp minimum is greater than maximum.");
            }
        }

        private static void ValidateFloatingBounds(double min, double max)
        {
            if (double.IsNaN(min) || double.IsNaN(max))
            {
                throw new InvalidOperationException("Clamp bounds cannot be NaN.");
            }

            if (min > max)
            {
                throw new InvalidOperationException("Clamp minimum is greater than maximum.");
            }
        }
    }
}