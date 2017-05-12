// Copyright(C) 2017, Alexander Verbeek

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Creates an abstract tree-like shape out of tetrahedrons (triangular pyramids) and renders it as a triangle mesh.
/// Every tetrahedron is attached to one or more other tetraherons. Two attached tetrahedrons share a common face.
/// Only faces that are 'outside' the shape will be rendered as triangles.
/// Different tetrahedrons that are not directly connected to one another, may intersect.
/// </summary>
public class TetrahedronTree : MonoBehaviour {

    [SerializeField]
    private int seed = 0;

    [SerializeField]
    private bool randomSeed = false;

    [SerializeField]
    private int maxDepth = 5;

    private TetrahedronTreeNode tree;
    private Mesh mesh;

    void Start() {
        MeshFilter filter = GetComponent<MeshFilter>();
        mesh = new Mesh();
        filter.sharedMesh = mesh;
        CreateTree();
    }

    // A very basic GUI for generating trees
    void OnGUI() {
        GUILayout.Window(0, new Rect(10, 10, 100, 100), OnWindow, "Tree Demo");
    }

    private void OnWindow(int id) {
        randomSeed = GUILayout.Toggle(randomSeed, "Random seed");
        string seedTxt = GUILayout.TextField(seed.ToString());
        int.TryParse(seedTxt, out seed);
        if (GUILayout.Button("Create Tree")) {
            CreateTree();
        }
    }

    // create tree and build mesh
    public void CreateTree() {
        tree = new TetrahedronTreeNode(
            new Vector3(0, 0, 0),
            new Vector3(1, 0, 0),
            new Vector3(0, 0, 1),
            new Vector3(0.25f, 1, 0.25f));

        if(randomSeed) {
            seed = Random.Range(int.MinValue, int.MaxValue);
        }

        Random.InitState(seed);
        TetrahedronTreeGenerator.GenerateTree(tree, maxDepth);
        RebuildTreeMesh(tree, mesh);
    }

    // fill a mesh object with triangle data.
    public static void RebuildTreeMesh(TetrahedronTreeNode tree, Mesh mesh) {

        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> indices = new List<int>();

        int vCount = 0;
        CreateMeshData(tree, vertices, normals, uvs, indices, ref vCount);
        mesh.Clear();
        mesh.SetVertices(vertices);
        mesh.SetNormals(normals);
        mesh.SetUVs(0, uvs);
        mesh.SetTriangles(indices, 0);
        mesh.UploadMeshData(false);
    }

    // Creates triangle mesh data for the specified node and its children by filling the given lists with triangle data.
    private static void CreateMeshData(TetrahedronTreeNode node, List<Vector3> vertices, List<Vector3> normals, List<Vector2> uvs, List<int> indices, ref int vCount) {
        for (int i = 0; i < 3; i++) {
            TetrahedronTreeNode child = node.children[i];
            if (child != null) {
                CreateMeshData(child, vertices, normals, uvs, indices, ref vCount);
            } else {
                // This triangle face does not have a child treenode, so let's draw it
                Vector3[] faceverts = node.GetFace(i);
                vertices.Add(faceverts[0]);
                vertices.Add(faceverts[1]);
                vertices.Add(faceverts[2]);

                Vector3 n = node.GetFaceNormal(i);
                normals.Add(n);
                normals.Add(n);
                normals.Add(n);

                // let's not care too much about uvs right now, so set it to zero. 
                // Maybe later, if we decide to do texturing mapping or other stuff.
                Vector2 uv = Vector2.zero;
                uvs.Add(uv);
                uvs.Add(uv);
                uvs.Add(uv);

                indices.Add(vCount + 0);
                indices.Add(vCount + 1);
                indices.Add(vCount + 2);
                vCount = vCount + 3;
            }
        }
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        DrawNodeGizmo(tree);
    }

    void DrawNodeGizmo(TetrahedronTreeNode node) {
        if (node == null)
            return;

        for (int i = 0; i < 3; i++) {
            Vector3 n = node.GetFaceNormal(i);
            Vector3 avg = node.GetFaceAvg(i);
            Gizmos.DrawLine(avg, avg + n * 0.1f);

            DrawNodeGizmo(node.children[i]);
        }

        for (int i = 0; i < 4; i++) {
            Vector3 v = node.vertices[i];
            Gizmos.DrawSphere(v, 0.01f);
        }
    }
}