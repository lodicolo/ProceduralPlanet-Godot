using System;
using Godot;
using Godot.Collections;
using Godot.Extensions;

namespace ProceduralPlanet.scripts.planet.shape.modules;

[Tool]
public partial class PerturbPointsModule : PerturbModule
{
    public override Vector3[] Run(Vector3[] vertices, float maxPerturbStrength) =>
        RunShader(
            "res://materials/shaders/compute/PerturbPoints.glsl",
            (renderingDevice, shaderId) =>
            {
                float[] shaderParams = { vertices.Length, maxPerturbStrength, };
                var shaderParamsBytes = shaderParams.ToByteArray();

                var verticesBytes = vertices.ToByteArray();
                using AutoFreeRid verticesBufferId = new(
                    renderingDevice,
                    renderingDevice.StorageBufferCreate((uint)verticesBytes.Length, verticesBytes)
                );

                RDUniform uniformVertices = new()
                {
                    UniformType = RenderingDevice.UniformType.StorageBuffer,
                    Binding = 0,
                };
                uniformVertices.AddId(verticesBufferId);

                using AutoFreeRid shaderParamsBufferId = new(
                    renderingDevice,
                    renderingDevice.StorageBufferCreate((uint)shaderParamsBytes.Length, shaderParamsBytes)
                );

                RDUniform uniformShaderParams = new()
                {
                    UniformType = RenderingDevice.UniformType.StorageBuffer,
                    Binding = 1,
                };
                uniformShaderParams.AddId(shaderParamsBufferId);

                using AutoFreeRid uniformSetId = new(
                    renderingDevice,
                    renderingDevice.UniformSetCreate(
                        new Array<RDUniform>(new[] { uniformVertices, uniformShaderParams }),
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

                var outputBytes = renderingDevice.BufferGetData(verticesBufferId);
                var output = outputBytes.ToTypedArray<float>();

                var outputVertices = new Vector3[vertices.Length];
                for (var index = 0; index < vertices.Length; ++index)
                {
                    var offset = index * 3;
                    outputVertices[index] = new Vector3(output[offset + 0], output[offset + 1], output[offset + 2]);
                }

                return outputVertices;
            }
        );
}