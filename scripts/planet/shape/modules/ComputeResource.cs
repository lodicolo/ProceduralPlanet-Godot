using System;
using System.Diagnostics;
using Godot;
using Godot.Extensions;

namespace ProceduralPlanet.scripts.planet.shape.modules;

[Tool]
public abstract partial class ComputeResource : Resource
{
    private RenderingDevice? _renderingDevice;

    protected string ResourceId => $"{GetType().Name}:{NativeInstance}";

    protected string SafeResourceName
    {
        get
        {
            try
            {
                return NativeInstance == default ? "DISPOSED" : ResourceName;
            }
            catch
            {
                return "DISPOSED_RACE_CONDITION";
            }
        }
    }

    protected string FullResourceId => $"{ResourceId} \"{SafeResourceName}\"";

    protected RenderingDevice RenderingDevice
    {
        get
        {
            if (_renderingDevice == default)
            {
                var renderingDevice = RenderingServer.CreateLocalRenderingDevice();
                var id = renderingDevice.NativeInstance;
                Debug.WriteLine($"{FullResourceId} create local rendering device {id}");
                _renderingDevice = renderingDevice;
            }

            return _renderingDevice;
        }
    }

    protected TOutput RunShader<TOutput>(string shaderName, Func<RenderingDevice, Rid, TOutput> runner)
    {
        using var shaderFile = GD.Load<RDShaderFile>(shaderName);
        using var shaderSpirV = shaderFile.GetSpirV();
        var renderingDevice = RenderingDevice;
        using AutoFreeRid shaderId = new(renderingDevice, renderingDevice.ShaderCreateFromSpirV(shaderSpirV));
        return runner(renderingDevice, shaderId);
    }

    protected override void Dispose(bool disposing)
    {
        Debug.WriteLine($"{FullResourceId} disposing ({disposing})");

        if (disposing)
        {
            (var renderingDevice, _renderingDevice) = (_renderingDevice, default);
            if (renderingDevice is { NativeInstance: var id } && id != default)
            {
                Debug.WriteLine($"{FullResourceId} free local rendering device {id}");
                renderingDevice.Free();
            }
            else
            {
                Debug.WriteLine($"{FullResourceId} local rendering device handle no longer valid");
            }

            renderingDevice?.Dispose();
        }

        base.Dispose(disposing);
    }
}