using System;
using Godot;

namespace ProceduralPlanet.Utilities;

public record struct AutoFreeRid(RenderingDevice RenderingDevice, Rid Rid) : IDisposable
{
    public Rid Rid { get; private set; } = Rid;

    public void Dispose()
    {
        if (Rid.IsValid)
        {
            RenderingDevice.FreeRid(Rid);
        }

        Rid = default;
    }

    public static implicit operator Rid(AutoFreeRid autoFreeRid) => autoFreeRid.Rid;
}