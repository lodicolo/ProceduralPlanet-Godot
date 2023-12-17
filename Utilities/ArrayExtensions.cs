using System;
using System.Runtime.InteropServices;

namespace ProceduralPlanet.Utilities;

public static class ArrayExtensions
{
    public static byte[] ToByteArray<T>(this T[] array, int offset = 0, int? count = null) where T : unmanaged
    {
        var safeOffset = Math.Max(0, offset);
        var safeCount = count ?? array.Length - safeOffset;

        var buffer = new byte[Marshal.SizeOf<T>() * safeCount];
        Buffer.BlockCopy(array, safeOffset, buffer, 0, safeCount);
        return buffer;
    }
}