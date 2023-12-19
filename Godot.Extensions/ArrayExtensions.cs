using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Godot.Extensions;

public static class ArrayExtensions
{
    private const string WarnValidationErrorFormat = "[WARN] [{0}] Validation error: {1}";

    public static bool ThrowOnError { get; set; } = false;

    public static bool ValidationWarnings { get; set; } = true;
    
    public static unsafe byte[] ToByteArray<T>(this T[] array, int offset = 0, int? count = null) where T : unmanaged
    {
        var safeOffset = Math.Max(0, offset);
        var tRemaining = array.Length - safeOffset;
        var safeCount = count ?? tRemaining;

        if (safeCount > tRemaining)
        {
            if (ValidationWarnings || ThrowOnError)
            {
                var message = $"Attempted to copy {safeCount} {typeof(T).Name} from {offset} ({tRemaining} remaining)";
                Console.Error.WriteLine(WarnValidationErrorFormat, nameof(ToByteArray), message);

                if (ThrowOnError || Debugger.IsAttached)
                {
                    throw new InvalidOperationException(message);
                }
            }

            safeCount = tRemaining;
        }

        switch (array)
        {
            case bool[]:
            case byte[]:
            case sbyte[]:
            {
                var clone = new byte[safeCount];
                Buffer.BlockCopy(array, safeOffset, clone, 0, safeCount);
                return clone;
            }

            case char[]:
            case short[]:
            case ushort[]:
            {
                if (1073741823 < safeCount)
                {
                    throw new InvalidOperationException(
                        $"Converting {safeCount} {typeof(T).Name} requires more bytes than can be in an array"
                    );
                }
                var clone = new byte[safeCount << 1];
                Buffer.BlockCopy(array, safeOffset, clone, 0, clone.Length);
                return clone;
            }

            case float[]:
            case int[]:
            case uint[]:
            {
                if (536870911 < safeCount)
                {
                    throw new InvalidOperationException(
                        $"Converting {safeCount} {typeof(T).Name} requires more bytes than can be in an array"
                    );
                }
                var clone = new byte[safeCount << 2];
                Buffer.BlockCopy(array, safeOffset, clone, 0, clone.Length);
                return clone;
            }

            case double[]:
            case long[]:
            case ulong[]:
            {
                if (268435455 < safeCount)
                {
                    throw new InvalidOperationException(
                        $"Converting {safeCount} {typeof(T).Name} requires more bytes than can be in an array"
                    );
                }
                var clone = new byte[safeCount << 3];
                Buffer.BlockCopy(array, safeOffset, clone, 0, clone.Length);
                return clone;
            }

            default:
                var sizeOfT = Marshal.SizeOf<T>();
                if (int.MaxValue / sizeOfT < safeCount)
                {
                    throw new InvalidOperationException(
                        $"Converting {safeCount} {typeof(T).Name} requires more bytes than can be in an array"
                    );
                }

                var buffer = new byte[sizeOfT * safeCount];
                fixed (T* typePointer = &array[0])
                {
                    var ptr = (nint)typePointer;
                    Marshal.Copy(ptr + safeOffset * sizeOfT, buffer, 0, buffer.Length);
                }
                return buffer;
        }
    }

    private static readonly System.Collections.Generic.Dictionary<Type, int> TypeSizes = new()
    {
        { typeof(bool), 1 },
        { typeof(char), 2 }
    };

    public static unsafe T[] ToTypedArray<T>(this byte[] buffer, int offset = 0, int? count = null) where T : unmanaged
    {
        var safeOffset = Math.Max(0, offset);
        var bytesRemaining = buffer.Length - safeOffset;
        var safeCount = count ?? bytesRemaining;

        if (safeCount > bytesRemaining)
        {
            if (ValidationWarnings || ThrowOnError)
            {
                var message = $"Attempted to copy {safeCount} bytes from {offset} ({bytesRemaining} remaining)";
                Console.Error.WriteLine(WarnValidationErrorFormat, nameof(ToTypedArray), message);

                if (ThrowOnError || Debugger.IsAttached)
                {
                    throw new InvalidOperationException(message);
                }
            }

            safeCount = bytesRemaining;
        }

        if (!TypeSizes.TryGetValue(typeof(T), out var sizeOfT))
        {
            sizeOfT = Marshal.SizeOf<T>();
        }

        if (safeCount < sizeOfT)
        {
            throw new InvalidOperationException(
                $"{typeof(T).Name} is {sizeOfT} bytes but the input buffer only contained {safeCount}"
            );
        }

        var countOfType = safeCount / sizeOfT;
        var diff = safeCount - countOfType * sizeOfT;

        if (diff != 0 && (ValidationWarnings || ThrowOnError))
        {
            var message = $"Trying to cast {safeCount} bytes to {countOfType} {typeof(T).Name} ({diff} bytes required)";
            Console.Error.WriteLine(WarnValidationErrorFormat, nameof(ToTypedArray), message);

            if (ThrowOnError || Debugger.IsAttached)
            {
                throw new InvalidOperationException(message);
            }
        }

        var typedBuffer = new T[countOfType];
        switch (typedBuffer)
        {
            case bool[]:
            case byte[]:
            case sbyte[]:
            case char[]:
            case short[]:
            case ushort[]:
            case float[]:
            case int[]:
            case uint[]:
            case double[]:
            case long[]:
            case ulong[]:
            {
                Buffer.BlockCopy(buffer, safeOffset, typedBuffer, 0, countOfType * sizeOfT);
                return typedBuffer;
            }

            default:
                fixed (T* typePointer = &typedBuffer[0])
                {
                    var ptr = (nint)typePointer;
                    Marshal.Copy(buffer, safeOffset, ptr, buffer.Length);
                }
                return typedBuffer;
        }

    }
}