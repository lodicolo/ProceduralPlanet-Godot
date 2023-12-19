using System.Numerics;

namespace Godot.Extensions.Tests;

public partial class ArrayExtensionsTests
{
    [Test]
    public void ToByteArray_Primitives_decimal()
    {
        Assert.That(
            new[] { decimal.Zero, decimal.One, decimal.MinusOne, decimal.MinValue, decimal.MaxValue }.ToByteArray(),
            Is.EqualTo(
                Enumerable.Empty<byte>()
                    .Concat(Enumerable.Repeat(byte.MinValue, 16))
                    .Concat(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0 })
                    .Concat(new byte[] { 0, 0, 0, 0x80, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0 })
                    .Concat(
                        new byte[]
                        {
                            0, 0, 0, 0x80, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff
                        }
                    )
                    .Concat(
                        new byte[]
                        {
                            0, 0, 0, 0, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff
                        }
                    )
                    .ToArray()
            )
        );
    }

    [Test]
    public void ToByteArray_Structs_Transform2D()
    {
        Assert.That(
            new[] { Transform2D.Identity, Transform2D.FlipX, Transform2D.FlipY, }.ToByteArray(),
            Is.EqualTo(
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
            )
        );
    }

    [Test]
    public void ToByteArray_Structs_Transform3D()
    {
        Assert.That(
            new[] { Transform3D.Identity, Transform3D.FlipX, Transform3D.FlipY, Transform3D.FlipZ }.ToByteArray(),
            Is.EqualTo(
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
            )
        );
    }

    [Test]
    public void ToByteArray_Structs_Projection()
    {
        Assert.That(
            new[] { Projection.Identity }.ToByteArray(),
            Is.EqualTo(
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
            )
        );
    }

    [Test]
    public void ToByteArray_Structs_Matrix3x2()
    {
        Assert.That(
            new[] { Matrix3x2.Identity }.ToByteArray(),
            Is.EqualTo(
                Enumerable.Empty<byte>()
                    .Concat(BitConverter.GetBytes(1f))
                    .Concat(BitConverter.GetBytes(0f))
                    .Concat(BitConverter.GetBytes(0f))
                    .Concat(BitConverter.GetBytes(1f))
                    .Concat(BitConverter.GetBytes(0f))
                    .Concat(BitConverter.GetBytes(0f))
                    .ToArray()
            )
        );
    }

    [Test]
    public void ToByteArray_Structs_Matrix4x4()
    {
        Assert.That(
            new[] { Matrix4x4.Identity }.ToByteArray(),
            Is.EqualTo(
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
            )
        );
    }

    [Test]
    public void ToByteArray_Structs_Vector2I()
    {
        Assert.That(
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
            }.ToByteArray(),
            Is.EqualTo(
                new byte[]
                {
                    0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x0f, 0x00, 0x00, 0x00, 0xff, 0x00, 0x00, 0x00,
                    0xff, 0x00, 0xff, 0x00, 0xf0, 0xff, 0x0f, 0x00,
                }
            )
        );
    }

    [Test]
    public void ToByteArray_Structs_Vector3I()
    {
        Assert.That(
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
            }.ToByteArray(),
            Is.EqualTo(
                new byte[]
                {
                    0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0xff, 0xff, 0xff, 0x7f, 0x0f, 0x00, 0x00, 0x00,
                    0xff, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x80, 0xff, 0x00, 0xff, 0x00, 0xf0, 0xff, 0x0f, 0x00,
                    0xff, 0xff, 0xff, 0xff,
                }
            )
        );
    }

    [Test]
    public void ToByteArray_Structs_Vector3()
    {
        Assert.That(
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
                    Y = float.NaN,
                    Z = float.NegativeInfinity,
                },
                new Vector3
                {
                    X = float.NegativeZero,
                    Y = float.PositiveInfinity,
                    Z = 1f,
                }
            }.ToByteArray(),
            Is.EqualTo(
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
            )
        );
    }
}