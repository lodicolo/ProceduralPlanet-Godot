using System.Numerics;

namespace Godot.Extensions.Tests;

public partial class ArrayExtensionsTests
{
    [Test]
    public void ToTypedArray_Primitives_decimal()
    {
        var inputArray = new[] { decimal.Zero, decimal.One, decimal.MinusOne, decimal.MinValue, decimal.MaxValue };

        Assert.That(
            Enumerable.Empty<byte>()
                .Concat(Enumerable.Repeat(byte.MinValue, 16))
                .Concat(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0 })
                .Concat(new byte[] { 0, 0, 0, 0x80, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0 })
                .Concat(
                    new byte[] { 0, 0, 0, 0x80, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff }
                )
                .Concat(
                    new byte[] { 0, 0, 0, 0, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff }
                )
                .ToArray()
                .ToTypedArray<decimal>(),
            Is.EqualTo(inputArray)
        );
    }

    [Test]
    public void ToTypedArray_Structs_Transform2D()
    {
        Assert.That(
            Enumerable.Empty<byte>()
                .Concat(BitConverter.GetBytes(1f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(1f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(-1f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(1f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(1f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(-1f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(0f))
                .ToArray()
                .ToTypedArray<Transform2D>(),
            Is.EqualTo(new[] { Transform2D.Identity, Transform2D.FlipX, Transform2D.FlipY, })
        );
    }

    [Test]
    public void ToTypedArray_Structs_Transform3D()
    {
        Assert.That(
            Enumerable.Empty<byte>()
                .Concat(BitConverter.GetBytes(1f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(1f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(1f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(-1f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(1f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(1f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(1f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(-1f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(1f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(1f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(1f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(-1f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(0f))
                .ToArray()
                .ToTypedArray<Transform3D>(),
            Is.EqualTo(new[] { Transform3D.Identity, Transform3D.FlipX, Transform3D.FlipY, Transform3D.FlipZ })
        );
    }

    [Test]
    public void ToTypedArray_Structs_Projection()
    {
        Assert.That(
            Enumerable.Empty<byte>()
                .Concat(BitConverter.GetBytes(1f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(1f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(1f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(1f))
                .ToArray()
                .ToTypedArray<Projection>(),
            Is.EqualTo(new[] { Projection.Identity })
        );
    }

    [Test]
    public void ToTypedArray_Structs_Matrix3x2()
    {
        Assert.That(
            Enumerable.Empty<byte>()
                .Concat(BitConverter.GetBytes(1f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(1f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(0f))
                .ToArray()
                .ToTypedArray<Matrix3x2>(),
            Is.EqualTo(new[] { Matrix3x2.Identity })
        );
    }

    [Test]
    public void ToTypedArray_Structs_Matrix4x4()
    {
        Assert.That(
            Enumerable.Empty<byte>()
                .Concat(BitConverter.GetBytes(1f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(1f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(1f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(1f))
                .ToArray()
                .ToTypedArray<Matrix4x4>(),
            Is.EqualTo(new[] { Matrix4x4.Identity })
        );
    }

    [Test]
    public void ToTypedArray_Structs_Vector2I()
    {
        Assert.That(
            new byte[]
            {
                0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x0f, 0x00, 0x00, 0x00, 0xff, 0x00, 0x00, 0x00,
                0xff, 0x00, 0xff, 0x00, 0xf0, 0xff, 0x0f, 0x00,
            }.ToTypedArray<Vector2I>(),
            Is.EqualTo(
                new[]
                {
                    new Vector2I
                    {
                        X = 0,
                        Y = 1,
                    },
                    new Vector2I
                    {
                        X = 0xf,
                        Y = 0xff,
                    },
                    new Vector2I
                    {
                        X = 0xff00ff,
                        Y = 0x0ffff0,
                    }
                }
            )
        );
    }

    [Test]
    public void ToTypedArray_Structs_Vector3I()
    {
        Assert.That(
            new byte[]
            {
                0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0xff, 0xff, 0xff, 0x7f, 0x0f, 0x00, 0x00, 0x00,
                0xff, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x80, 0xff, 0x00, 0xff, 0x00, 0xf0, 0xff, 0x0f, 0x00,
                0xff, 0xff, 0xff, 0xff,
            }.ToTypedArray<Vector3I>(),
            Is.EqualTo(
                new[]
                {
                    new Vector3I
                    {
                        X = 0,
                        Y = 1,
                        Z = int.MaxValue,
                    },
                    new Vector3I
                    {
                        X = 0xf,
                        Y = 0xff,
                        Z = int.MinValue,
                    },
                    new Vector3I
                    {
                        X = 0xff00ff,
                        Y = 0x0ffff0,
                        Z = -1,
                    }
                }
            )
        );
    }

    [Test]
    public void ToTypedArray_Structs_Vector3()
    {
        Assert.That(
            Enumerable.Empty<byte>()
                .Concat(BitConverter.GetBytes(0f))
                .Concat(BitConverter.GetBytes(float.MinValue))
                .Concat(BitConverter.GetBytes(float.MaxValue))
                .Concat(BitConverter.GetBytes(float.E))
                .Concat(BitConverter.GetBytes(float.Epsilon))
                .Concat(BitConverter.GetBytes(float.Pi))
                .Concat(BitConverter.GetBytes(float.Tau))
                .Concat(BitConverter.GetBytes(0f)) // NaN fails IsEquals
                .Concat(BitConverter.GetBytes(float.NegativeInfinity))
                .Concat(BitConverter.GetBytes(float.NegativeZero))
                .Concat(BitConverter.GetBytes(float.PositiveInfinity))
                .Concat(BitConverter.GetBytes(1f))
                .ToArray()
                .ToTypedArray<Vector3>(),
            Is.EqualTo(
                new[]
                {
                    new Vector3
                    {
                        X = 0f,
                        Y = float.MinValue,
                        Z = float.MaxValue,
                    },
                    new Vector3
                    {
                        X = float.E,
                        Y = float.Epsilon,
                        Z = float.Pi,
                    },
                    new Vector3
                    {
                        X = float.Tau,
                        Y = 0f, // NaN fails IsEquals
                        Z = float.NegativeInfinity,
                    },
                    new Vector3
                    {
                        X = float.NegativeZero,
                        Y = float.PositiveInfinity,
                        Z = 1f,
                    }
                }
            )
        );
    }
}