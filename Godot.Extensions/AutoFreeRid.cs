using System;

namespace Godot.Extensions;

public record struct AutoFreeRid(RenderingDevice RenderingDevice, Rid Rid) : IDisposable
{
    public Rid Rid { get; private set; } = Rid;

    public void Dispose()
    {
        if (Rid.IsValid)
        {
            // Console.WriteLine($"Freeing {Rid}");
            RenderingDevice.FreeRid(Rid);
        }
        else
        {
            Console.WriteLine($"Skipping free of {Rid} because it is no longer valid");
        }

        Rid = default;
    }

    public static implicit operator Rid(AutoFreeRid autoFreeRid) => autoFreeRid.Rid;
}