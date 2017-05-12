// Copyright(C) 2017, Alexander Verbeek

using UnityEngine;

/// <summary>
/// Generates pseudo-randomly shaped trees according to a set of rules.
/// </summary>
public class TetrahedronTreeGenerator {

    public static void GenerateTree(TetrahedronTreeNode node, int maxDepth) {
        float chance = BranchChance(node);

        if (maxDepth <= 0) return;

        // for each face of this node
        for (int i = 0; i < 3; i++) {
            // determine if we should branch or not
            if (i == 0 || Random.value < chance) {
                // start a new branch by creating a new TetTreeNode
                // get vertices of base face
                Vector3[] verts = node.GetFace(i);

                // determine the position of the last vertex -and the overall shape- for the new tetrahedron.
                // (this to a large extend determines how the final tree will look like)
                Vector3 basepos = node.GetFaceAvg(i);
                Vector3 dir = BranchLength(node) * (node.dir + node.GetFaceNormal(i));
                float maxDev = BranchMaxDeviation(node);
                // rotate direction vector some random amount
                Quaternion rot = Quaternion.Euler(Random.value * maxDev, Random.value * maxDev, Random.value * maxDev);
                dir = rot * dir;
                // apply upwards tendency, because we want this to look like a tree and not a shrubbery.
                dir.y += BranchUpwardsStrength(node);
                Vector3 d = basepos + dir;

                // create the new tree node and continue generating
                TetrahedronTreeNode child = new TetrahedronTreeNode(verts[2], verts[1], verts[0], d);
                node.children[i] = child;
                GenerateTree(child, maxDepth - 1);
            }
        }
    }

    private static float BranchChance(TetrahedronTreeNode node) {
        return 0.075f * node.basepos.y;
    }

    private static float BranchLength(TetrahedronTreeNode node) {
        return 1;
    }

    private static float BranchMaxDeviation(TetrahedronTreeNode node) {
        return 10f * 0.1f * node.basepos.y;
    }

    private static float BranchUpwardsStrength(TetrahedronTreeNode node) {
        return Mathf.Clamp01(-0.5f * node.basepos.y + 1);
    }
}
