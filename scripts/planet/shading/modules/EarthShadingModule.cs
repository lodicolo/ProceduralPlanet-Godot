using System;
using System.Linq;
using Godot;
using Godot.Collections;
using ProceduralPlanet.scripts.planet.settings.noise_settings;
using Godot.Extensions;
using Array = System.Array;

namespace ProceduralPlanet.scripts.planet.shading.modules;

[Tool]
public partial class EarthShadingModule : ShadingDataModule
{
    private readonly SimplexNoiseSettings _warp2Noise = new();
    private readonly SimplexNoiseSettings _noise2Noise = new();

    [Export] public SimplexNoiseSettings? DetailNoise { get; set; }
    [Export] public SimplexNoiseSettings? DetailWarpNoise { get; set; }
    [Export] public SimplexNoiseSettings? LargeNoise { get; set; }
    [Export] public SimplexNoiseSettings? SmallNoise { get; set; }

    public override UVPairs Run(RandomNumberGenerator rng, Vector3[] vertices)
    {
        var renderingDevice = RenderingDevice;

        using var shaderFile = GD.Load<RDShaderFile>("res://materials/shaders/compute/EarthShading.glsl");
        using var shaderSpirV = shaderFile.GetSpirV();
        using AutoFreeRid shaderId = new(renderingDevice, renderingDevice.ShaderCreateFromSpirV(shaderSpirV));

        var verticesBuffer = vertices.ToByteArray();

        var shadingData = new float[vertices.Length * 4];
        var shadingDataBuffer = shadingData.ToByteArray();

        var shaderParams = new float[] { vertices.Length };
        var shaderParamsBuffer = shaderParams.ToByteArray();

        var noiseParams = Enumerable.Empty<float>()
            .Concat(DetailWarpNoise?.GetParams(rng) ?? Array.Empty<float>())
            .Concat(DetailNoise?.GetParams(rng) ?? Array.Empty<float>())
            .Concat(LargeNoise?.GetParams(rng) ?? Array.Empty<float>())
            .Concat(SmallNoise?.GetParams(rng) ?? Array.Empty<float>())
            .Concat(_warp2Noise.GetParams(rng))
            .Concat(_noise2Noise.GetParams(rng))
            .ToArray();
        var noiseParamsBuffer = noiseParams.ToByteArray();

        using AutoFreeRid verticesBufferId = new(
            renderingDevice,
            renderingDevice.StorageBufferCreate((uint)verticesBuffer.Length, verticesBuffer)
        );
        RDUniform uniformVertices = new()
        {
            UniformType = RenderingDevice.UniformType.StorageBuffer,
            Binding = 0,
        };
        uniformVertices.AddId(verticesBufferId);

        using AutoFreeRid shadingDataBufferId = new(
            renderingDevice,
            renderingDevice.StorageBufferCreate((uint)shadingDataBuffer.Length, shadingDataBuffer)
        );
        RDUniform uniformShadingData = new()
        {
            UniformType = RenderingDevice.UniformType.StorageBuffer,
            Binding = 1,
        };
        uniformShadingData.AddId(shadingDataBufferId);

        using AutoFreeRid paramsBufferId = new(
            renderingDevice,
            renderingDevice.StorageBufferCreate((uint)shaderParamsBuffer.Length, shaderParamsBuffer)
        );
        RDUniform uniformParams = new()
        {
            UniformType = RenderingDevice.UniformType.StorageBuffer,
            Binding = 2,
        };
        uniformParams.AddId(paramsBufferId);

        using AutoFreeRid noiseParamsBufferId = new(
            renderingDevice,
            renderingDevice.StorageBufferCreate((uint)noiseParamsBuffer.Length, noiseParamsBuffer)
        );
        RDUniform uniformNoiseParams = new()
        {
            UniformType = RenderingDevice.UniformType.StorageBuffer,
            Binding = 3,
        };
        uniformNoiseParams.AddId(noiseParamsBufferId);

        using AutoFreeRid uniformSetId = new(
            renderingDevice,
            renderingDevice.UniformSetCreate(
                new Array<RDUniform>(new[] { uniformVertices, uniformShadingData, uniformParams, uniformNoiseParams }),
                shaderId,
                0
            )
        );

        using AutoFreeRid pipelineId = new(renderingDevice, renderingDevice.ComputePipelineCreate(shaderId));
        var computeListId = renderingDevice.ComputeListBegin();
        renderingDevice.ComputeListBindComputePipeline(computeListId, pipelineId);
        renderingDevice.ComputeListBindUniformSet(computeListId, uniformSetId, 0);
        var groups = (uint)Math.Ceiling(vertices.Length / 512f);
        renderingDevice.ComputeListDispatch(computeListId, groups, 1, 1);
        renderingDevice.ComputeListEnd();

        renderingDevice.Submit();
        renderingDevice.Sync();

        var outputBytes = renderingDevice.BufferGetData(shadingDataBufferId);
        var output = outputBytes.ToTypedArray<float>();

        var uv1 = new Vector2[vertices.Length];
        var uv2 = new Vector2[vertices.Length];
        for (var uvIndex = 0; uvIndex < uv1.Length; ++uvIndex)
        {
            var offset = uvIndex << 2;
            uv1[uvIndex] = new Vector2(output[offset + 0], output[offset + 1]);
            uv2[uvIndex] = new Vector2(output[offset + 2], output[offset + 3]);
        }

        return new UVPairs(uv1, uv2);
    }
}