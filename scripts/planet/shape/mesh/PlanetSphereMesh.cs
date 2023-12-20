using System;
using System.Collections.Generic;
using Godot;

namespace ProceduralPlanet.scripts.planet.shape.mesh;

public record Edge(int[] Indices);

[Tool]
public partial class PlanetSphereMesh : Resource
{

    private static readonly Vector3[] BaseVertices = {
        Vector3.Up, Vector3.Left, Vector3.Back, Vector3.Right, Vector3.Forward, Vector3.Down,
    };

    private static readonly int[] EdgeTriplets =
    {
        0, 1, 4, 1, 2, 5, 2, 3, 6, 3, 0, 7, 8, 9, 4, 9, 10, 5, 10, 11, 6, 11, 8, 7
    };

    private static readonly int[] VertexPairs =
    {
        0, 1, 0, 2, 0, 3, 0, 4, 1, 2, 2, 3, 3, 4, 4, 1, 5, 1, 5, 2, 5, 3, 5, 4
    };

    private int _numDivisions;
    private int _numVerticesPerFace;

    public PlanetSphereMesh(int resolution)
    {
        Resolution = resolution;

        _numDivisions = Math.Max(0, resolution);

        var ndp3 = _numDivisions + 3;
        _numVerticesPerFace = (int)(Math.Pow(ndp3, 2) - ndp3) / 2;

        List<int> indices = new();
        List<Vector3> vertices = new();

        vertices.AddRange(BaseVertices);

        var edges = new Edge[12];
        var vertexPairs = VertexPairs;
        for (var vpIndex = 0; vpIndex < vertexPairs.Length; vpIndex += 2)
        {
            var vpStart = vertexPairs[vpIndex];
            var vpEnd = vertexPairs[vpIndex + 1];

            var startVertex = vertices[vpStart];
            var endVertex = vertices[vpEnd];

            var edgeVertexIndices = new int[_numDivisions + 2];
            edgeVertexIndices[0] = vpStart;

            for (var divisionIndex = 0; divisionIndex < _numDivisions; ++divisionIndex)
            {
                var t = (divisionIndex + 1f) / (_numDivisions + 1f);
                edgeVertexIndices[divisionIndex + 1] = vertices.Count;
                vertices.Add(startVertex.Slerp(endVertex, t));
            }

            edgeVertexIndices[_numDivisions + 1] = vpEnd;
            var edgeIndex = vpIndex >> 1;
            edges[edgeIndex] = new Edge(edgeVertexIndices);
        }

        var edgeTriplets = EdgeTriplets;
        for (var tripletIndex = 0; tripletIndex < edgeTriplets.Length; tripletIndex += 3)
        {
            var faceIndex = tripletIndex / 3;
            var reverse = faceIndex > 3;
            CreateFace(
                vertices,
                indices,
                _numDivisions,
                edges[edgeTriplets[tripletIndex + 0]],
                edges[edgeTriplets[tripletIndex + 1]],
                edges[edgeTriplets[tripletIndex + 2]],
                reverse
            );
        }

        Vertices = vertices.ToArray();
        Indices = indices.ToArray();
    }

    public int[] Indices { get; private set; }

    public int Resolution { get; private set; }

    public Vector3[] Vertices { get; private set; }

    private static void CreateFace(
        List<Vector3> vertices,
        List<int> indices,
        int numDivisions,
        Edge sideA,
        Edge sideB,
        Edge bottom,
        bool reverse
    )
    {
        var numPointsInEdge = sideA.Indices.Length;
        List<int> vertexMap = new()
        {
            sideA.Indices[0],
        };

        for (var index = 1; index < numPointsInEdge - 1; ++index)
        {
            var sideAIndex = sideA.Indices[index];
            var sideBIndex = sideB.Indices[index];

            vertexMap.Add(sideAIndex);

            var sideAVertex = vertices[sideAIndex];
            var sideBVertex = vertices[sideBIndex];
            var numInnerPoints = index - 1;
            for (var indexOther = 0; indexOther < numInnerPoints; ++indexOther)
            {
                var t = (indexOther + 1f) / (numInnerPoints + 1f);
                vertexMap.Add(vertices.Count);
                vertices.Add(sideAVertex.Slerp(sideBVertex, t));
            }

            vertexMap.Add(sideBIndex);
        }

        for (var index = 0; index < numPointsInEdge; ++index)
        {
            vertexMap.Add(bottom.Indices[index]);
        }

        var numRows = numDivisions + 1;
        for (var row = 0; row < numRows; ++row)
        {
            var topVertex = ((row + 1) * (row + 1) - row - 1) / 2;
            var bottomVertex = ((row + 2) * (row + 2) - row - 2) / 2;

            var numTrianglesInRow = 1 + row << 1;
            for (var column = 0; column < numTrianglesInRow; ++column)
            {
                var v0 = topVertex;
                var v1 = bottomVertex;
                var v2 = topVertex - 1;

                if (column % 2 == 0)
                {
                    v2 = v1;
                    ++v1;
                    ++topVertex;
                    ++bottomVertex;
                }

                indices.Add(vertexMap[v0]);
                if (reverse)
                {
                    indices.Add(vertexMap[v2]);
                    indices.Add(vertexMap[v1]);
                }
                else
                {
                    indices.Add(vertexMap[v1]);
                    indices.Add(vertexMap[v2]);
                }
            }
        }
    }
}