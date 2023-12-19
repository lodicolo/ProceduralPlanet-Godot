using System;
using Godot;
using ProceduralPlanet.Utilities;

namespace ProceduralPlanet.scripts.planet.shape.modules;

[Tool]
public abstract partial class ComputeResource : Resource
{
    private RenderingDevice? _renderingDevice;

    protected RenderingDevice RenderingDevice => _renderingDevice ??= RenderingServer.CreateLocalRenderingDevice();

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
        base.Dispose(disposing);

        (var renderingDevice, _renderingDevice) = (_renderingDevice, default);
        renderingDevice?.Free();
        renderingDevice?.Dispose();
    }
}