using System.Collections;

namespace Godot.Extensions.Tests;

public partial class ArrayExtensionsTests
{
    [Test]
    public void ToTypedArray_Primitives_bool()
    {
        Assert.That(
            new byte[] { 0, 1, 0, 0, 1, 1, 1, 1, 0, 0, 1, 0, 1, 0, 0, 0, 1, 0, 0, 1, 1, 1, 1, 0, 0, 1, 1, 0, 0, 0, }
                .ToTypedArray<bool>(),
            Is.EqualTo(
                new[]
                {
                    false, true, false, false, true, true, true, true, false, false, true, false, true, false,
                    false, false, true, false, false, true, true, true, true, false, false, true, true, false,
                    false, false
                }
            )
        );
    }

    [Test]
    public void ToTypedArray_Primitives_byte()
    {
        var inputArray = Enumerable.Range(0, 256).Select(i => unchecked((byte)i)).ToArray();
        Assert.That(
            new byte[]
            {
                0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26,
                27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51,
                52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76,
                77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100,
                101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120,
                121, 122, 123, 124, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 138, 139, 140,
                141, 142, 143, 144, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160,
                161, 162, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175, 176, 177, 178, 179, 180,
                181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 192, 193, 194, 195, 196, 197, 198, 199, 200,
                201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220,
                221, 222, 223, 224, 225, 226, 227, 228, 229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239, 240,
                241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252, 253, 254, 255
            }.ToTypedArray<byte>(),
            Is.EqualTo(inputArray)
        );
    }

    [Test]
    public void ToTypedArray_Primitives_sbyte()
    {
        var inputArray = Enumerable.Range(0, 256).Select(i => unchecked((sbyte)(i + sbyte.MinValue))).ToArray();
        Assert.That(
            new byte[]
            {
                128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147,
                148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 164, 165, 166, 167,
                168, 169, 170, 171, 172, 173, 174, 175, 176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 186, 187,
                188, 189, 190, 191, 192, 193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207,
                208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227,
                228, 229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239, 240, 241, 242, 243, 244, 245, 246, 247,
                248, 249, 250, 251, 252, 253, 254, 255, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
                17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41,
                42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66,
                67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91,
                92, 93, 94, 95, 96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112,
                113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 124, 125, 126, 127
            }.ToTypedArray<sbyte>(),
            Is.EqualTo(inputArray)
        );
    }

    private static IEnumerable ToTypedArray_Primitives_char_cases()
    {
        yield return CreateParams("Hello, world!");
        yield return CreateParams("Лорем ипсум долор");
        yield return CreateParams("農あスつで稿告キ");
        yield return CreateParams("국군의 조직과 편");
        yield return CreateParams("مكن اليها الحكومة التغييرات");
        yield return CreateParams("ב נפלו תאולוגיה אחד, בה");
        yield break;

        static object[] CreateParams(string input)
        {
            return new object[]
            {
                input.SelectMany(c => new[] { (byte)(c & 0xff), (byte)((c >> 8) & 0xff) }).ToArray(),
                input.ToArray(),
            };
        }
    }

    [TestCaseSource(typeof(ArrayExtensionsTests), nameof(ToTypedArray_Primitives_char_cases))]
    public void ToTypedArray_Primitives_char(byte[] inputArray, char[] expected)
    {
        Assert.That(inputArray.ToTypedArray<char>(), Is.EqualTo(expected));
    }

    [Test]
    public void ToTypedArray_Primitives_double()
    {
        var inputArray = new[]
        {
            0.0, double.MinValue, double.MaxValue, double.E, double.Epsilon, double.Pi, double.Tau, double.NaN,
            double.NegativeInfinity, double.NegativeZero, double.PositiveInfinity, 1.0,
        };

        Assert.That(
            Enumerable.Empty<byte>()
                .Concat(BitConverter.GetBytes(0.0))
                .Concat(BitConverter.GetBytes(double.MinValue))
                .Concat(BitConverter.GetBytes(double.MaxValue))
                .Concat(BitConverter.GetBytes(double.E))
                .Concat(BitConverter.GetBytes(double.Epsilon))
                .Concat(BitConverter.GetBytes(double.Pi))
                .Concat(BitConverter.GetBytes(double.Tau))
                .Concat(BitConverter.GetBytes(double.NaN))
                .Concat(BitConverter.GetBytes(double.NegativeInfinity))
                .Concat(BitConverter.GetBytes(double.NegativeZero))
                .Concat(BitConverter.GetBytes(double.PositiveInfinity))
                .Concat(BitConverter.GetBytes(1.0))
                .ToArray()
                .ToTypedArray<double>(),
            Is.EqualTo(inputArray)
        );
    }

    [Test]
    public void ToTypedArray_Primitives_float()
    {
        var inputArray = new[]
        {
            0f, float.MinValue, float.MaxValue, float.E, float.Epsilon, float.Pi, float.Tau, float.NaN,
            float.NegativeInfinity, float.NegativeZero, float.PositiveInfinity, 1f,
        };

        Assert.That(
            Enumerable.Empty<byte>()
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(float.MinValue))
                .Concat(BitConverter.GetBytes(float.MaxValue))
                .Concat(BitConverter.GetBytes(float.E))
                .Concat(BitConverter.GetBytes(float.Epsilon))
                .Concat(BitConverter.GetBytes(float.Pi))
                .Concat(BitConverter.GetBytes(float.Tau))
                .Concat(BitConverter.GetBytes(float.NaN))
                .Concat(BitConverter.GetBytes(float.NegativeInfinity))
                .Concat(BitConverter.GetBytes(float.NegativeZero))
                .Concat(BitConverter.GetBytes(float.PositiveInfinity))
                .Concat(BitConverter.GetBytes(1f))
                .ToArray()
                .ToTypedArray<float>(),
            Is.EqualTo(inputArray)
        );
    }

    [Test]
    public void ToTypedArray_Primitives_short()
    {
        short[] inputArray =
        {
            0x0000, 0x000f, 0x00ff, 0x00f0, 0x0ff0, 0x0f00, unchecked((short)0xff00), unchecked((short)0xf000),
            unchecked((short)0xf00f), unchecked((short)0xffff),
        };

        Assert.That(
            new byte[]
            {
                0x00, 0x00, 0x0f, 0x00, 0xff, 0x00, 0xf0, 0x00, 0xf0, 0x0f, 0x00, 0x0f, 0x00, 0xff, 0x00, 0xf0,
                0x0f, 0xf0, 0xff, 0xff,
            }.ToTypedArray<short>(),
            Is.EqualTo(inputArray)
        );
    }

    [Test]
    public void ToTypedArray_Primitives_ushort()
    {
        ushort[] inputArray = { 0x0000, 0x000f, 0x00ff, 0x00f0, 0x0ff0, 0x0f00, 0xff00, 0xf000, 0xf00f, 0xffff, };

        Assert.That(
            new byte[]
            {
                0x00, 0x00, 0x0f, 0x00, 0xff, 0x00, 0xf0, 0x00, 0xf0, 0x0f, 0x00, 0x0f, 0x00, 0xff, 0x00, 0xf0,
                0x0f, 0xf0, 0xff, 0xff,
            }.ToTypedArray<ushort>(),
            Is.EqualTo(inputArray)
        );
    }

    [Test]
    public void ToTypedArray_Primitives_int()
    {
        int[] inputArray =
        {
            0x0, 0xff, 0xff00, 0xff0000, unchecked((int)0xff000000), unchecked((int)0xfedcba98), 0xf00f, 0xf00f0f,
            unchecked((int)0xf00f0ff0)
        };

        Assert.That(
            new byte[]
            {
                0x0, 0x0, 0x0, 0x0, 0xff, 0x0, 0x0, 0x0, 0x0, 0xff, 0x0, 0x0, 0x0, 0x0, 0xff, 0x0, 0x0, 0x0, 0x0,
                0xff, 0x98, 0xba, 0xdc, 0xfe, 0x0f, 0xf0, 0x0, 0x0, 0x0f, 0x0f, 0xf0, 0x0, 0xf0, 0x0f, 0x0f, 0xf0,
            }.ToTypedArray<int>(),
            Is.EqualTo(inputArray)
        );
    }

    [Test]
    public void ToTypedArray_Primitives_uint()
    {
        uint[] inputArray = { 0x0, 0xff, 0xff00, 0xff0000, 0xff000000, 0xfedcba98, 0xf00f, 0xf00f0f, 0xf00f0ff0 };

        Assert.That(
            new byte[]
            {
                0x0, 0x0, 0x0, 0x0, 0xff, 0x0, 0x0, 0x0, 0x0, 0xff, 0x0, 0x0, 0x0, 0x0, 0xff, 0x0, 0x0, 0x0, 0x0,
                0xff, 0x98, 0xba, 0xdc, 0xfe, 0x0f, 0xf0, 0x0, 0x0, 0x0f, 0x0f, 0xf0, 0x0, 0xf0, 0x0f, 0x0f, 0xf0,
            }.ToTypedArray<uint>(),
            Is.EqualTo(inputArray)
        );
    }

    [Test]
    public void ToTypedArray_Primitives_long()
    {
        long[] inputArray =
        {
            0x0, 0xf, 0xf0, 0xf00, 0xf000, 0xf0000, 0xf00000, 0xf000000, 0xf0000000, 0xf00000000, 0xf000000000,
            0xf0000000000, 0xf00000000000, 0xf000000000000, 0xf0000000000000, 0xf00000000000000,
            unchecked((long)0xf000000000000000), 0xfedcba98, 0xf00f, 0xf00f0f, 0xf00f0ff0,
            unchecked((long)0xfedcba9876543210), unchecked((long)0xffffffffffffffff),
            unchecked((long)0xffffffff00000000), 0x00000000ffffffff,
        };

        Assert.That(
            new byte[]
            {
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0f, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0xf0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0f, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0xf0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0f, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0xf0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0f, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0xf0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0f, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0xf0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0f, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0xf0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0f, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xf0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0f,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xf0, 0x98, 0xba, 0xdc, 0xfe, 0x00, 0x00, 0x00, 0x00,
                0x0f, 0xf0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0f, 0x0f, 0xf0, 0x00, 0x00, 0x00, 0x00, 0x00,
                0xf0, 0x0f, 0x0f, 0xf0, 0x00, 0x00, 0x00, 0x00, 0x10, 0x32, 0x54, 0x76, 0x98, 0xba, 0xdc, 0xfe,
                0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0x00, 0x00, 0x00, 0x00, 0xff, 0xff, 0xff, 0xff,
                0xff, 0xff, 0xff, 0xff, 0x00, 0x00, 0x00, 0x00,
            }.ToTypedArray<long>(),
            Is.EqualTo(inputArray)
        );
    }

    [Test]
    public void ToTypedArray_Primitives_ulong()
    {
        ulong[] inputArray =
        {
            0x0, 0xf, 0xf0, 0xf00, 0xf000, 0xf0000, 0xf00000, 0xf000000, 0xf0000000, 0xf00000000, 0xf000000000,
            0xf0000000000, 0xf00000000000, 0xf000000000000, 0xf0000000000000, 0xf00000000000000, 0xf000000000000000,
            0xfedcba98, 0xf00f, 0xf00f0f, 0xf00f0ff0, 0xfedcba9876543210, 0xffffffffffffffff, 0xffffffff00000000,
            0x00000000ffffffff,
        };

        Assert.That(
            new byte[]
            {
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0f, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0xf0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0f, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0xf0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0f, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0xf0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0f, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0xf0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0f, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0xf0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0f, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0xf0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0f, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xf0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0f,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xf0, 0x98, 0xba, 0xdc, 0xfe, 0x00, 0x00, 0x00, 0x00,
                0x0f, 0xf0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0f, 0x0f, 0xf0, 0x00, 0x00, 0x00, 0x00, 0x00,
                0xf0, 0x0f, 0x0f, 0xf0, 0x00, 0x00, 0x00, 0x00, 0x10, 0x32, 0x54, 0x76, 0x98, 0xba, 0xdc, 0xfe,
                0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0x00, 0x00, 0x00, 0x00, 0xff, 0xff, 0xff, 0xff,
                0xff, 0xff, 0xff, 0xff, 0x00, 0x00, 0x00, 0x00,
            }.ToTypedArray<ulong>(),
            Is.EqualTo(inputArray)
        );
    }
}