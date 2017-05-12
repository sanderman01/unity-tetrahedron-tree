// Copyright(C) 2017, Alexander Verbeek

using UnityEngine;

/// <summary>
/// A tree structure of tetrahedrons (triangular pyramids)
/// Each child node represents potentially another tetrahedron attached to a face of the parent tetrahedron.
/// </summary>
public class TetrahedronTreeNode {

    // Vertices of the tetrahedron
    public readonly Vector3[] vertices;

    /// <summary>
    /// The average position of the first three triangles, which form the 'base face' of the triangle 
    /// </summary>
    public readonly Vector3 basepos;
    /// <summary>
    /// dir is the direction from the 'base face' to the opposite vertex.
    /// </summary>
    public readonly Vector3 dir;

    // References to child trees attached to the faces of this tetrahedron.
    public TetrahedronTreeNode[] children;

    public TetrahedronTreeNode(Vector3 a, Vector3 b, Vector3 c, Vector3 d) {
        this.vertices = new Vector3[] { a, b, c, d };
        this.basepos = (a + b + c) * (1f / 3f);
        this.dir = (d - basepos).normalized;
        this.children = new TetrahedronTreeNode[3];
    }

    public Vector3[] GetFace(int i) {
        return new Vector3[] { vertices[i], vertices[3], vertices[(1 + i) % 3] };
    }

    public Vector3 GetFaceNormal(int i) {
        return GetFaceNormal(GetFace(i));
    }

    public static Vector3 GetFaceNormal(Vector3[] verts) {
        Vector3 a = verts[1] - verts[0];
        Vector3 b = verts[1] - verts[2];
        return Vector3.Cross(b, a).normalized;
    }

    public Vector3 GetFaceAvg(int i) {
        return GetFaceAvg(GetFace(i));
    }

    public static Vector3 GetFaceAvg(Vector3[] verts) {
        return (verts[0] + verts[1] + verts[2]) * (1f / 3f);
    }
}