using System;
using System.Linq;
using Godot;
using Godot.Collections;
using ProceduralPlanet.scripts.planet.settings.noise_settings;
using Godot.Extensions;
using static Godot.Extensions.PropertyHelper;
using Array = System.Array;

namespace ProceduralPlanet.scripts.planet.shape.modules;

[Tool]
public partial class EarthHeightModule : HeightModule
{
    private SimplexNoiseSettings? _continentsNoise;
    private float _oceanDepthMultiplier = 5;
    private float _oceanFloorDepth = 1.5f;
    private float _oceanFloorSmoothing = 0.5f;
    private SimplexNoiseSettings? _maskNoise;
    private float _mountainBlend = 1.2f;
    private RidgeNoiseSettings? _mountainsNoise;

    [Export]
    public SimplexNoiseSettings? ContinentsNoise
    {
        get => _continentsNoise;
        set
        {
            if (!SetIfChanged(ref _continentsNoise, value, out var lastValue))
            {
                return;
            }

            if (lastValue is not null)
            {
                lastValue.Changed -= EmitChanged;
            }

            EmitChanged();

            if (_continentsNoise is { } currentValue)
            {
                currentValue.Changed += EmitChanged;
            }
        }
    }

    [Export]
    public float OceanDepthMultiplier
    {
        get => _oceanDepthMultiplier;
        set => SetIfChanged(ref _oceanDepthMultiplier, value, EmitChanged);
    }

    [Export]
    public float OceanFloorDepth
    {
        get => _oceanFloorDepth;
        set => SetIfChanged(ref _oceanFloorDepth, value, EmitChanged);
    }

    [Export]
    public float OceanFloorSmoothing
    {
        get => _oceanFloorSmoothing;
        set => SetIfChanged(ref _oceanFloorSmoothing, value, EmitChanged);
    }

    [Export]
    public SimplexNoiseSettings? MaskNoise
    {
        get => _maskNoise;
        set
        {
            if (!SetIfChanged(ref _maskNoise, value, out var lastValue))
            {
                return;
            }

            if (lastValue is not null)
            {
                lastValue.Changed -= EmitChanged;
            }

            EmitChanged();

            if (_maskNoise is { } currentValue)
            {
                currentValue.Changed += EmitChanged;
            }
        }
    }

    [Export]
    public float MountainBlend
    {
        get => _mountainBlend;
        set => SetIfChanged(ref _mountainBlend, value, EmitChanged);
    }

    [Export]
    public RidgeNoiseSettings? MountainsNoise
    {
        get => _mountainsNoise;
        set
        {
            if (!SetIfChanged(ref _mountainsNoise, value, out var lastValue))
            {
                return;
            }

            if (lastValue is not null)
            {
                lastValue.Changed -= EmitChanged;
            }

            EmitChanged();

            if (_mountainsNoise is { } currentValue)
            {
                currentValue.Changed += EmitChanged;
            }
        }
    }

    public override float[] Run(RandomNumberGenerator rng, Vector3[] vertices) =>
        RunShader(
            "res://materials/shaders/compute/EarthHeight.glsl",
            (renderingDevice, shaderId) =>
            {
                // TODO: Is this really needed?
                var heights = new float[vertices.Length];
                var heightsBytes = heights.ToByteArray();

                float[] shaderParams =
                {
                    vertices.Length,
                    _oceanDepthMultiplier,
                    _oceanFloorDepth,
                    _oceanFloorSmoothing,
                    _mountainBlend,
                };
                var shaderParamsBytes = shaderParams.ToByteArray();

                var noiseParams = Enumerable.Empty<float>()
                    .Concat(_continentsNoise?.GetParams(rng) ?? Array.Empty<float>())
                    .Concat(_maskNoise?.GetParams(rng) ?? Array.Empty<float>())
                    .Concat(_mountainsNoise?.GetParams(rng) ?? Array.Empty<float>())
                    .ToArray();
                var noiseParamsBytes = noiseParams.ToByteArray();

                var verticesBytes = vertices.ToByteArray();
                AutoFreeRid verticesBufferId = new(
                    renderingDevice,
                    renderingDevice.StorageBufferCreate((uint)verticesBytes.Length, verticesBytes)
                );
                RDUniform uniformVertices = new()
                {
                    UniformType = RenderingDevice.UniformType.StorageBuffer,
                    Binding = 0,
                };
                uniformVertices.AddId(verticesBufferId);

                AutoFreeRid heightsBufferId = new(
                    renderingDevice,
                    renderingDevice.StorageBufferCreate((uint)heightsBytes.Length, heightsBytes)
                );
                RDUniform uniformHeights = new()
                {
                    UniformType = RenderingDevice.UniformType.StorageBuffer,
                    Binding = 1,
                };
                uniformHeights.AddId(heightsBufferId);

                AutoFreeRid shaderParamsBufferId = new(
                    renderingDevice,
                    renderingDevice.StorageBufferCreate((uint)shaderParamsBytes.Length, shaderParamsBytes)
                );
                RDUniform uniformShaderParams = new()
                {
                    UniformType = RenderingDevice.UniformType.StorageBuffer,
                    Binding = 2,
                };
                uniformShaderParams.AddId(shaderParamsBufferId);

                AutoFreeRid noiseParamsBufferId = new(
                    renderingDevice,
                    renderingDevice.StorageBufferCreate((uint)noiseParamsBytes.Length, noiseParamsBytes)
                );
                RDUniform uniformNoiseParams = new()
                {
                    UniformType = RenderingDevice.UniformType.StorageBuffer,
                    Binding = 3,
                };
                uniformNoiseParams.AddId(noiseParamsBufferId);

                AutoFreeRid uniformSetId = new(
                    renderingDevice,
                    renderingDevice.UniformSetCreate(
                        new Array<RDUniform>(
                            new[] { uniformVertices, uniformHeights, uniformShaderParams, uniformNoiseParams }
                        ),
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

                var outputBytes = renderingDevice.BufferGetData(heightsBufferId);
                var output = outputBytes.ToTypedArray<float>();

                return output;
            }
        );
}