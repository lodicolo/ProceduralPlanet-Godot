using System;
using System.Diagnostics;
using Godot;
using Godot.Collections;
using Godot.Extensions;
using static Godot.Extensions.PropertyHelper;

namespace ProceduralPlanet.scripts.planet.settings;

[Tool]
public partial class AtmosphereSettings : SettingsResource
{
    private bool _dirty = true;

    private float _atmosphereScale = 0.5f;
    private Texture2D? _blueNoiseTexture;
    private float _densityFalloff = 0.25f;
    private float _ditherScale = 4;
    private float _ditherStrength = 0.8f;
    private int _inScatteringPoints = 10;
    private float _intensity = 1;
    private int _opticalDepthPoints = 10;
    private ImageTexture? _opticalDepthTexture;
    private float _scatteringStrength = 20;
    private int _textureSize = 256;
    private Vector3 _wavelengths = new(700, 530, 460);

    [Export(PropertyHint.Range, "0,1")]
    public float AtmosphereScale
    {
        get => _atmosphereScale;
        set
        {
            if (SetIfChanged(ref _atmosphereScale, value, ref _dirty))
            {
                EmitChanged();
            }
        }
    }

    [Export]
    public Texture2D? BlueNoiseTexture
    {
        get => _blueNoiseTexture;
        set
        {
            if (SetIfChanged(ref _blueNoiseTexture, value, ref _dirty))
            {
                EmitChanged();
            }
        }
    }

    [Export]
    public float DensityFalloff
    {
        get => _densityFalloff;
        set
        {
            if (SetIfChanged(ref _densityFalloff, value, ref _dirty))
            {
                EmitChanged();
            }
        }
    }

    [Export]
    public float DitherScale
    {
        get => _ditherScale;
        set
        {
            if (SetIfChanged(ref _ditherScale, value, ref _dirty))
            {
                EmitChanged();
            }
        }
    }

    [Export]
    public float DitherStrength
    {
        get => _ditherStrength;
        set
        {
            if (SetIfChanged(ref _ditherStrength, value, ref _dirty))
            {
                EmitChanged();
            }
        }
    }

    [Export]
    public int InScatteringPoints
    {
        get => _inScatteringPoints;
        set
        {
            if (SetIfChanged(ref _inScatteringPoints, value, ref _dirty))
            {
                EmitChanged();
            }
        }
    }

    [Export]
    public float Intensity
    {
        get => _intensity;
        set
        {
            if (SetIfChanged(ref _intensity, value, ref _dirty))
            {
                EmitChanged();
            }
        }
    }

    [Export]
    public int OpticalDepthPoints
    {
        get => _opticalDepthPoints;
        set
        {
            if (SetIfChanged(ref _opticalDepthPoints, value, ref _dirty))
            {
                EmitChanged();
            }
        }
    }

    [Export]
    public ImageTexture? OpticalDepthTexture
    {
        get => _opticalDepthTexture ?? new ImageTexture();
        set
        {
            if (SetIfChanged(ref _opticalDepthTexture, value, ref _dirty))
            {
                EmitChanged();
            }
        }
    }

    [Export]
    public float ScatteringStrength
    {
        get => _scatteringStrength;
        set
        {
            if (SetIfChanged(ref _scatteringStrength, value, ref _dirty))
            {
                EmitChanged();
            }
        }
    }

    [Export]
    public int TextureSize
    {
        get => _textureSize;
        set
        {
            if (SetIfChanged(ref _textureSize, value, ref _dirty))
            {
                EmitChanged();
            }
        }
    }

    [Export]
    public Vector3 Wavelengths
    {
        get => _wavelengths;
        set
        {
            if (SetIfChanged(ref _wavelengths, value, ref _dirty))
            {
                EmitChanged();
            }
        }
    }

    private readonly RandomNumberGenerator _rng = new();

    private RenderingDevice? _renderingDevice;

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        (var renderingDevice, _renderingDevice) = (_renderingDevice, default);
        renderingDevice?.Free();
        renderingDevice?.Dispose();
    }

    public void SetProperties(ShaderMaterial? material, float bodyRadius)
    {
        var atmosphereRadius = (1 + _atmosphereScale) * bodyRadius;

        material?.SetShaderParameter("NumInScatteringPoints", _inScatteringPoints);
        material?.SetShaderParameter("NumOpticalDepthPoints", _opticalDepthPoints);
        material?.SetShaderParameter("AtmosphereRadius", atmosphereRadius);
        material?.SetShaderParameter("PlanetRadius", bodyRadius);
        material?.SetShaderParameter("DensityFalloff", _densityFalloff);

        var scaw = Vector3.One * 400 / _wavelengths;
        scaw.X = MathF.Pow(scaw.X, 4);
        scaw.Y = MathF.Pow(scaw.Y, 4);
        scaw.Z = MathF.Pow(scaw.Z, 4);

        var scatteringCoefficients = Vector3.One * 400 / _wavelengths;
        scatteringCoefficients *= scatteringCoefficients; // v^2
        scatteringCoefficients *= scatteringCoefficients; // (v^2)^2 aka v^4
        scatteringCoefficients *= _scatteringStrength;
        material?.SetShaderParameter("ScatteringCoefficients", scatteringCoefficients);
        material?.SetShaderParameter("Intensity", _intensity);
        material?.SetShaderParameter("DitherStrength", _ditherStrength);
        material?.SetShaderParameter("DitherScale", _ditherScale);
        material?.SetShaderParameter("BlueNoise", _blueNoiseTexture ?? default(Variant));

        if (_dirty)
        {
            _dirty = false;
            ComputeOpticalDepthTexture();
        }

        if (_opticalDepthTexture is { } opticalDepthTexture)
        {
            material?.SetShaderParameter("BakedOpticalDepth", opticalDepthTexture);
        }
        else
        {
            Debug.WriteLine("Missing computed optical depth texture");
            material?.SetShaderParameter("BakedOpticalDepth", default);
        }
    }

    private void ComputeOpticalDepthTexture()
    {
        try
        {
            Debug.WriteLine("Computing optical depth texture...");

            var renderingDevice = _renderingDevice ??= RenderingServer.CreateLocalRenderingDevice();

            using var shaderFile = GD.Load<RDShaderFile>("res://materials/shaders/compute/AtmosphereTexture.glsl");
            using var shaderSpirV = shaderFile.GetSpirV();

            using AutoFreeRid shaderId = new(renderingDevice, renderingDevice.ShaderCreateFromSpirV(shaderSpirV));

            var sizeU32 = (uint)_textureSize;
            var sizeI32 = _textureSize;

            RDTextureFormat format = new()
            {
                Width = sizeU32,
                Height = sizeU32,
                Format = RenderingDevice.DataFormat.R32G32B32A32Sfloat,
                UsageBits = RenderingDevice.TextureUsageBits.StorageBit |
                            RenderingDevice.TextureUsageBits.CanUpdateBit |
                            RenderingDevice.TextureUsageBits.CanCopyFromBit,
                TextureType = RenderingDevice.TextureType.Type2D,
            };

            RDTextureView textureView = new();

            using AutoFreeRid textureId = new(renderingDevice, renderingDevice.TextureCreate(format, textureView));

            float[] inputParams = { _textureSize, _opticalDepthPoints, 1 + _atmosphereScale, _densityFalloff, };
            var inputParamsBuffer = inputParams.ToByteArray();

            RDUniform uniformTexture = new()
            {
                UniformType = RenderingDevice.UniformType.Image,
                Binding = 0,
            };
            uniformTexture.AddId(textureId);

            using AutoFreeRid inputParamsStorageBufferId = new(
                renderingDevice,
                renderingDevice.StorageBufferCreate((uint)inputParamsBuffer.Length, inputParamsBuffer)
            );
            RDUniform uniformParams = new()
            {
                UniformType = RenderingDevice.UniformType.StorageBuffer,
                Binding = 1,
            };
            uniformParams.AddId(inputParamsStorageBufferId);

            using AutoFreeRid uniformSetId = new(
                renderingDevice,
                renderingDevice.UniformSetCreate(
                    new Array<RDUniform>(new[] { uniformTexture, uniformParams }),
                    shaderId,
                    0
                )
            );

            using AutoFreeRid pipelineId = new(renderingDevice, renderingDevice.ComputePipelineCreate(shaderId));

            var computeListId = renderingDevice.ComputeListBegin();
            renderingDevice.ComputeListBindComputePipeline(computeListId, pipelineId);
            renderingDevice.ComputeListBindUniformSet(computeListId, uniformSetId, 0);
            var groups = (uint)Math.Ceiling(sizeU32 / 16f);
            renderingDevice.ComputeListDispatch(computeListId, groups, groups, 1);
            renderingDevice.ComputeListEnd();

            renderingDevice.Submit();
            renderingDevice.Sync();

            var outputBytes = renderingDevice.TextureGetData(textureId, 0);

            var image = Image.CreateFromData(sizeI32, sizeI32, false, Image.Format.Rgbaf, outputBytes);
            var newOpticalDepthTexture = ImageTexture.CreateFromImage(image);
            (var oldOpticalDepthTexture, _opticalDepthTexture) = (_opticalDepthTexture, newOpticalDepthTexture);
            oldOpticalDepthTexture?.Dispose();
        }
        catch (Exception exception)
        {
            Debug.WriteLine(exception.Message);
            throw;
        }
    }
}