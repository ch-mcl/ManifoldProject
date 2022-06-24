using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    public class TempMeshVis : MonoBehaviour
    {
        public GfzTrackSegment trackSegment;
        public int widthSamples = 4;
        public float minDistance = 10f;

        private void OnDrawGizmos()
        {
            if (trackSegment is null)
                return;

            var tempMesh = TrackGeoGenerator.GenerateTopSurface(trackSegment, minDistance, widthSamples);
            var mesh = new Mesh();
            var submesh = CreateSubMesh(tempMesh, mesh);
            mesh.SetSubMesh(0, submesh, MeshUpdateFlags.Default);

            Gizmos.color = Color.green;
            for (int i = 0; i < mesh.subMeshCount; i++)
                Gizmos.DrawMesh(mesh, i);

            Gizmos.color = Color.black;
            for (int vi = 0; vi < mesh.vertices.Length - 1; vi++)
            {
                var v0 = mesh.vertices[vi];
                var v1 = mesh.vertices[vi + 1];
                Gizmos.DrawLine(v0, v1);
            }
        }

        public static SubMeshDescriptor CreateSubMesh(TempMesh tempMesh, Mesh mesh)
        {
            var submesh = new SubMeshDescriptor();

            // New from this list/submesh
            //var vertices = tempMesh.vertexes;
            //var normals = GetNormals(displayList.nrm, nVerts);
            //var uv1 = GetUVs(displayList.tex0, nVerts);
            //var uv2 = GetUVs(displayList.tex1, nVerts);
            //var uv3 = GetUVs(displayList.tex2, nVerts);
            //var colors = GetColors(displayList.clr0, nVerts);
            //var triangles = GetTrianglesFromTriangleStrip(vertices.Length, isCCW);

            // Build submesh
            submesh.baseVertex = 0;// mesh.vertexCount;
            submesh.firstVertex = 0;// mesh.vertexCount;
            submesh.indexCount = tempMesh.indexes.Length;// triangles.Length;
            submesh.indexStart = 0;// mesh.triangles.Length;
            submesh.topology = MeshTopology.Triangles;
            submesh.vertexCount = tempMesh.vertexes.Length;// vertices.Length;

            // Append to mesh
            //var verticesConcat = mesh.vertices.Concat(vertices).ToArray();
            //var normalsConcat = mesh.normals.Concat(normals).ToArray();
            //var uv1Concat = mesh.uv.Concat(uv1).ToArray();
            //var uv2Concat = mesh.uv2.Concat(uv2).ToArray();
            //var uv3Concat = mesh.uv3.Concat(uv3).ToArray();
            //var colorsConcat = mesh.colors32.Concat(colors).ToArray();
            ////if (list.nbt != null)
            ////    mesh.tangents = list.nbt;
            //var trianglesConcat = mesh.triangles.Concat(triangles).ToArray();

            //// Assign values to mesh
            //mesh.vertices = verticesConcat;
            //mesh.normals = normalsConcat;
            //mesh.uv = uv1Concat;
            //mesh.uv2 = uv2Concat;
            //mesh.uv3 = uv3Concat;
            //mesh.colors32 = colorsConcat;
            //mesh.triangles = trianglesConcat;

            mesh.vertices = tempMesh.vertexes;
            mesh.normals = tempMesh.normals;
            mesh.triangles = tempMesh.indexes;

            return submesh;
        }
    }
}
