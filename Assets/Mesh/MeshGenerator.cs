using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour {
    public float minHeightForSecretPassage = 14.99f;
    public float secretPassageInnerHeight = .2f;
    public float secretPassageOuterHeight = .4f;
    public float secretPassageOuterWidth = 1/6f;
    public int[] corners = {1, 1, 1, 1, 1, 1};

    private Mesh mesh;
    private int[] triangles;
    private Vector3[] vertices;
    private List<int>[] sideNormals;

    private float scale;
    private static float SQRT3_4 = Mathf.Sqrt(3) / 4;
    private static float SQRT_1_3_4 = Mathf.Sqrt(1/3f) / 4;
    private float rampPermitted;

    public Corners Corners { get => new Corners(corners); }
    public Vector3 position { get => transform.position; }

    void Start() {
        UpdateMesh();
    }

    private void UpdateMesh() {
        if (mesh == null) {
            scale = Terrain.I.scale;
            rampPermitted = scale;

            mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = mesh;
        }

        GenerateMesh();
        UpdateUnityMesh();
    }

    public int SetCornerReturningDiff(int i, int newLevel) {
        corners[i] = newLevel;
        return SetCornersReturningDiff(corners);
    }

    public int SetCornersClampReturningDiff(int[] corners, bool raiseFloor) {
        if (raiseFloor) {
            int floor = Mathf.Max(corners) - Terrain.Scale * 2 + 1;
            for (int i = 0; i < 6; i++) if (corners[i] < floor) corners[i] = floor;
        } else {
            int ceil = Mathf.Min(corners) + Terrain.Scale * 2 - 1;
            for (int i = 0; i < 6; i++) if (corners[i] > ceil) corners[i] = ceil;
        }
        return SetCornersReturningDiff(corners);
    }

    private int SetCornersReturningDiff(int[] corners) {
        return SetCornersReturningDiff(corners[0], corners[1], corners[2], corners[3], corners[4], corners[5]);
    }

    public int SetCornersReturningDiff(int a, int b, int c, int d, int e, int f) {
        int diff = Mathf.Min(a, b, c, d, e, f) - 1;
        this.corners = new int[] {a - diff, b - diff, c - diff, d - diff, e - diff, f - diff};
        UpdateMesh();
        return diff;
    }

    private void GenerateMesh() {
        List<int> tris = new List<int>();
        List<Vector3> verts = new List<Vector3>();
        sideNormals = new List<int>[] {
            new List<int>(),
            new List<int>(),
            new List<int>(),
            new List<int>(),
            new List<int>(),
            new List<int>()
        };
        
        int minCorner = Mathf.Min(corners);
        float avgSum = 0;
        int avgCount = 0;
        for (int i = 0; i < 6; i++) if (corners[i] - minCorner < Terrain.I.scale) {
            avgSum += corners[i];
            avgCount++;
        }
        verts.Add(new Vector3(0, avgSum / avgCount / scale, 0));

        AddCorner(verts, 0);
        int firstCorner = verts.Count - 1;
        int prevCorner = firstCorner;

        for (int i = 0; i < 5; i++) {
            AddCorner(verts, i + 1);
            int nextCorner = verts.Count - 1;
            AddSide(verts, tris, i, prevCorner, nextCorner);
            prevCorner = nextCorner;
        }
        AddSide(verts, tris, 5, prevCorner, firstCorner);

        triangles = tris.ToArray();
        vertices = verts.ToArray();
    }

    private Quaternion Rot(float i) => Quaternion.Euler(0, i * -60 + 60, 0);

    private bool HasGap(int i, float middle) {
        return corners[i] / scale - middle >= .999f;
    }

    private int[] Tri(bool flip, int a, int b, int c) =>
        flip ? new int [] {a, c, b} : new int[] {a, b, c};

    private void MarkVertex(List<Vector3> verts, List<int> markedVerts) {
        markedVerts.Add(verts.Count - 1);
    }

    private void AddCorner(List<Vector3> verts, int i) {
        float middle = verts[0].y;

        if (!HasGap(i, middle)) {
            verts.Add(Rot(i) * new Vector3(0, corners[i] / scale, .75f)); // step      c - 3
            verts.Add(Rot(i) * new Vector3(0, corners[i] / scale, .875f)); // outer rim c - 2
            verts.Add(Rot(i) * new Vector3(0, corners[i] / scale, .875f)); // outer rim c - 1
            // MarkVertex(verts, upNormals);
            verts.Add(Rot(i) * new Vector3(0, 0, .875f)); // outer base                 c
        } else {
            verts.Add(Rot(i) * new Vector3(0, middle, .375f)); // inner base            c - 6 middle
            verts.Add(Rot(i) * new Vector3(0, middle, .375f)); // inner base            c - 5 facade
            verts.Add(Rot(i) * new Vector3(0, corners[i] / scale, .375f)); // inner rim c - 4 facade
            verts.Add(Rot(i) * new Vector3(0, corners[i] / scale, .375f)); // inner rim c - 3 top
            verts.Add(Rot(i) * new Vector3(0, corners[i] / scale, .875f)); // outer rim     c - 2 top
            verts.Add(Rot(i) * new Vector3(0, corners[i] / scale, .875f)); // outer rim     c - 1 outer
            // MarkVertex(verts, upNormals);
            verts.Add(Rot(i) * new Vector3(0, 0, .875f)); // outer base                     c     outer
        }
    }

    private void AddSide(List<Vector3> verts, List<int> tris, int i0, int c0, int c1) {
        float middle = verts[0].y;

        bool gap0 = HasGap(i0, middle);
        int i1 = (i0 + 1) % 6;
        int last = verts.Count - 1;
        bool gap1 = HasGap(i1, middle);

        bool secretPassage = gap0 && gap1
            && corners[i0] - middle * scale >= minHeightForSecretPassage
            && corners[i1] - middle * scale >= minHeightForSecretPassage;

        if (!gap0 && !gap1) {
            int c4 = last + 3;
            int c5 = c4 + 3;
            AddCircularCorner(verts, tris, i0, c0, c1, c4, c5);
            tris.AddRange(new int[] {c4, c4 - 1, c5}); // outer face
            tris.AddRange(new int[] {c5, c4 - 1, c5 - 1}); // outer face
            tris.AddRange(new int[] {c5 - 2, c4 - 2, c1 - 3}); // step
            tris.AddRange(new int[] {c1 - 3, c4 - 2, c0 - 3}); // step
            tris.AddRange(new int[] {c1 - 3, c0 - 3, 0}); // middle
        } else if (secretPassage) { 
            verts.Add(Rot(i0 + .5f) * new Vector3(0, (corners[i0] + corners[i1]) / 2f / scale, SQRT3_4)); // last + 1
            verts.Add(Rot(i0 + .5f) * new Vector3(0, (corners[i0] + corners[i1]) / 2f / scale, SQRT3_4)); // last + 2
            // odds are wall, evens are ground and passage walls
            verts.Add(Rot(i0 + .5f) * new Vector3(0, middle + secretPassageInnerHeight, SQRT3_4)); // last + 3
            verts.Add(Rot(i0 + .5f) * new Vector3(0, middle + secretPassageInnerHeight, SQRT3_4)); // last + 4
            verts.Add(Rot(i0) * new Vector3(-SQRT_1_3_4, middle, .375f)); // last + 5
            verts.Add(Rot(i0) * new Vector3(-SQRT_1_3_4, middle, .375f)); // last + 6
            verts.Add(Rot(i1) * new Vector3(SQRT_1_3_4, middle, .375f)); // last + 7
            verts.Add(Rot(i1) * new Vector3(SQRT_1_3_4, middle, .375f)); // last + 8
            verts.Add(Rot(i0 + .5f) * new Vector3(0, middle + secretPassageOuterHeight, SQRT3_4 * 2)); // last + 9
            verts.Add(Rot(i0 + .5f) * new Vector3(0, middle + secretPassageOuterHeight, SQRT3_4 * 2)); // last + 10
            verts.Add(Rot(i0 + .5f) * new Vector3(secretPassageOuterWidth, middle, SQRT3_4 * 2)); // last + 11
            verts.Add(Rot(i0 + .5f) * new Vector3(secretPassageOuterWidth, middle, SQRT3_4 * 2)); // last + 12
            verts.Add(Rot(i0 + .5f) * new Vector3(-secretPassageOuterWidth, middle, SQRT3_4 * 2)); // last + 13
            verts.Add(Rot(i0 + .5f) * new Vector3(-secretPassageOuterWidth, middle, SQRT3_4 * 2)); // last + 14
            int c4 = last + 14 + 3;
            int c5 = c4 + 3;
            AddCircularCorner(verts, tris, i0, c0, c1, c4, c5);
            tris.AddRange(new int[] {c4, last + 11, c5}); // 7 outer faces
            tris.AddRange(new int[] {c5, last + 11, last + 13});
            tris.AddRange(new int[] {c4, c4 - 1, last + 11});
            tris.AddRange(new int[] {last + 11, c4 - 1, last + 9});
            tris.AddRange(new int[] {c5 - 1, c5, last + 13});
            tris.AddRange(new int[] {last + 13, last + 9, c5 - 1});
            tris.AddRange(new int[] {c5 - 1, last + 9, c4 - 1});
            tris.AddRange(new int[] {c4 - 2, c0 - 3, last + 1}); // top
            tris.AddRange(new int[] {last + 1, c5 - 2, c4 - 2}); // top
            tris.AddRange(new int[] {c5 - 2, last + 1, c1 - 3}); // top
            tris.AddRange(new int[] {last + 2, c0 - 4, last + 3}); // inner facade
            tris.AddRange(new int[] {last + 3, c0 - 4, last + 5}); // inner facade
            tris.AddRange(new int[] {last + 5, c0 - 4, c0 - 5}); // inner facade
            tris.AddRange(new int[] {c1 - 5, c1 - 4, last + 7}); // inner facade
            tris.AddRange(new int[] {last + 7, c1 - 4, last + 3}); // inner facade
            tris.AddRange(new int[] {last + 3, c1 - 4, last + 2}); // inner facade
            tris.AddRange(new int[] {c0 - 6, 0, last + 6}); // middle
            tris.AddRange(new int[] {last + 6, 0, last + 8}); // middle
            tris.AddRange(new int[] {last + 8, 0, c1 - 6}); // middle
            tris.AddRange(new int[] {last + 6, last + 8, last + 12}); // passage floor
            tris.AddRange(new int[] {last + 12, last + 8, last + 14}); // passage floor
            tris.AddRange(new int[] {last + 6, last + 12, last + 4}); // passage walls
            tris.AddRange(new int[] {last + 4, last + 12, last + 10}); // passage walls
            tris.AddRange(new int[] {last + 10, last + 14, last + 4}); // passage walls
            tris.AddRange(new int[] {last + 4, last + 14, last + 8}); // passage walls
        } else if (gap0 && gap1) {
            verts.Add(Rot(i0 + .5f) * new Vector3(0, (corners[i0] + corners[i1]) / 2f / scale, SQRT3_4)); // last + 1
            verts.Add(Rot(i0 + .5f) * new Vector3(0, (corners[i0] + corners[i1]) / 2f / scale, SQRT3_4)); // last + 2
            verts.Add(Rot(i0 + .5f) * new Vector3(0, middle, SQRT3_4));                                // last + 3
            verts.Add(Rot(i0 + .5f) * new Vector3(0, middle, SQRT3_4));                                // last + 4
            int c4 = last + 4 + 3;
            int c5 = c4 + 3;
            AddCircularCorner(verts, tris, i0, c0, c1, c4, c5);
            tris.AddRange(new int[] {c4, c4 - 1, c5}); // outer face
            tris.AddRange(new int[] {c5, c4 - 1, c5 - 1}); // outer face
            tris.AddRange(new int[] {c4 - 2, c0 - 3, last + 1}); // top
            tris.AddRange(new int[] {last + 1, c5 - 2, c4 - 2}); // top
            tris.AddRange(new int[] {c5 - 2, last + 1, c1 - 3}); // top
            tris.AddRange(new int[] {last + 2, c0 - 4, c0 - 5}); // inner facade
            tris.AddRange(new int[] {last + 2, c0 - 5, last + 3}); // inner facade
            tris.AddRange(new int[] {c1 - 5, c1 - 4, last + 2}); // inner facade
            tris.AddRange(new int[] {c1 - 5, last + 2, last + 3}); // inner facade
            tris.AddRange(new int[] {c0 - 6, 0, last + 4}); // middle
            tris.AddRange(new int[] {last + 4, 0, c1 - 6}); // middle
        } else if (Mathf.Abs(corners[i0] - corners[i1]) < rampPermitted) {
            int i2 = gap0 ? i1 : i0; // no gap
            int i3 = gap0 ? i0 : i1; // gap
            int c2 = gap0 ? c1 : c0; // no gap
            int c3 = gap0 ? c0 : c1; // gap
            int pinwheel = last + 1;
            verts.Add(Rot(i0 + .5f) * new Vector3(0, (corners[i0] + corners[i1]) / 2f / scale, SQRT3_4)); // pinwheel point
            verts.Add(Rot(i0 + .5f) * new Vector3(0, (corners[i0] + corners[i1]) / 2f / scale, SQRT3_4)); // pinwheel point for inner facade
            AddCircularCorner(verts, tris, i0, c0, c1, last + 2 + 3, last + 2 + 6);
            int c4 = gap0 ? last + 2 + 6 : last + 2 + 3;
            int c5 = gap0 ? last + 2 + 3 : last + 2 + 6;
            tris.AddRange(Tri(gap0, c4, c4 - 1, c5)); // outer face
            tris.AddRange(Tri(gap0, c5, c4 - 1, c5 - 1)); // outer face
            tris.AddRange(Tri(gap0, c5 - 2, pinwheel, c3 - 3)); // gap top
            tris.AddRange(Tri(gap0, c4 - 2, pinwheel, c5 - 2)); // outer top
            tris.AddRange(Tri(gap0, c2 - 3, pinwheel, c4 - 2)); // step
            tris.AddRange(Tri(gap0, 0, pinwheel, c2 - 3)); // nongap step to middle
            tris.AddRange(Tri(gap0, c3 - 6, pinwheel, 0)); // middle transition
            tris.AddRange(Tri(gap0, c3 - 4, last + 2, c3 - 5)); // inner facade
        } else {
            int i2 = gap0 ? i1 : i0; // no gap
            int i3 = gap0 ? i0 : i1; // gap
            int c2 = gap0 ? c1 : c0; // no gap
            int c3 = gap0 ? c0 : c1; // gap
            int pinwheelInner = last + 6;
            int pinwheelBase = last + 7;
            verts.Add(Rot(i0 + .5f) * new Vector3(0, corners[i3] / scale, SQRT3_4 * 2)); // outer last + 1
            // MarkVertex(verts, upNormals);
            verts.Add(Rot(i0 + .5f) * new Vector3(0, corners[i3] / scale, SQRT3_4 * 2)); // top   last + 2
            verts.Add(Rot(i0 + .5f) * new Vector3(0, corners[i3] / scale, SQRT3_4 * 2)); // inner last + 3
            verts.Add(Rot(i0 + .5f) * new Vector3(0, corners[i3] / scale, SQRT3_4));     // top   last + 4
            verts.Add(Rot(i0 + .5f) * new Vector3(0, corners[i3] / scale, SQRT3_4));     // inner last + 5
            verts.Add(Rot(i0 + .5f) * new Vector3(0, middle, SQRT3_4));                  // inner last + 6 pinwheel
            verts.Add(Rot(i0 + .5f) * new Vector3(0, middle, SQRT3_4));                  // base  last + 7 pinwheel
            verts.Add(Rot(i0 + .5f) * new Vector3(0, corners[i2] / scale, SQRT3_4 * 2)); // base  last + 8
            verts.Add(Rot(i0 + .5f) * new Vector3(0, corners[i2] / scale, SQRT3_4 * 2)); // inner last + 9
            verts.Add(Rot(i0 + .5f) * new Vector3(0, corners[i2] / scale, SQRT3_4 * 2)); // outer last + 10
            // MarkVertex(verts, upNormals);
            AddCircularCorner(verts, tris, i0, c0, c1, last + 10 + 3, last + 10 + 6);
            int c4 = gap0 ? last + 10 + 6 : last + 10 + 3;
            int c5 = gap0 ? last + 10 + 3 : last + 10 + 6;
            tris.AddRange(Tri(gap0, c4 - 1, last + 10, c4)); // outer face
            tris.AddRange(Tri(gap0, c4, last + 10, c5)); // outer face
            tris.AddRange(Tri(gap0, last + 1, c5, last + 10)); // outer face
            tris.AddRange(Tri(gap0, c5 - 1, c5, last + 1)); // outer face
            tris.AddRange(Tri(gap0, last + 2, last + 4, c5 - 2)); // gap top
            tris.AddRange(Tri(gap0, c5 - 2, last + 4, c3 - 3)); // gap top
            tris.AddRange(Tri(gap0, last + 9, pinwheelInner, last + 3)); // facade end
            tris.AddRange(Tri(gap0, last + 3, pinwheelInner, last + 5)); // facade end
            tris.AddRange(Tri(gap0, last + 5, pinwheelInner, c3 - 4)); // inner facade
            tris.AddRange(Tri(gap0, c3 - 4, pinwheelInner, c3 - 5)); // inner facade
            tris.AddRange(Tri(gap0, c3 - 6, pinwheelBase, 0)); // middle
            tris.AddRange(Tri(gap0, 0, pinwheelBase, c2 - 3)); // nongap to middle
            tris.AddRange(Tri(gap0, c2 - 3, pinwheelBase, last + 8)); // outer top
            tris.AddRange(Tri(gap0, last + 8, c4 - 2, c2 - 3)); // step
        }
    }

    private void AddCircularCorner(List<Vector3> verts, List<int> tris, int i0, int c0, int c1, int c4, int c5) {
        int i1 = (i0 + 1) % 6;
        verts.Add(Rot(i0 + .5f) * new Vector3(.25f, corners[i0] / scale, SQRT3_4 * 2)); // circular rim  c4 - 2 circle + 6
        verts.Add(Rot(i0 + .5f) * new Vector3(.25f, corners[i0] / scale, SQRT3_4 * 2)); // circular rim  c4 - 1 circle + 4
        MarkVertex(verts, sideNormals[i0]);
        verts.Add(Rot(i0 + .5f) * new Vector3(.25f, 0, SQRT3_4 * 2)); // circular base                   c4     circle + 2
        MarkVertex(verts, sideNormals[i0]);
        verts.Add(Rot(i0 + .5f) * new Vector3(-.25f, corners[i1] / scale, SQRT3_4 * 2)); // circular rim c5 - 2 circle + 5
        verts.Add(Rot(i0 + .5f) * new Vector3(-.25f, corners[i1] / scale, SQRT3_4 * 2)); // circular rim c5 - 1 circle + 3
        MarkVertex(verts, sideNormals[i0]);
        verts.Add(Rot(i0 + .5f) * new Vector3(-.25f, 0, SQRT3_4 * 2)); // circular base                  c5     circle + 1
        MarkVertex(verts, sideNormals[i0]);
        tris.AddRange(new int[] {c4, c0, c4 - 1}); // circular outer face
        tris.AddRange(new int[] {c4 - 1, c0, c0 - 1}); // circular outer face
        tris.AddRange(new int[] {c0 - 2, c0 - 3, c4 - 2}); // circular top
        tris.AddRange(new int[] {c1, c5, c5 - 1}); // circular outer face
        tris.AddRange(new int[] {c5 - 1, c1 - 1, c1}); // circular outer face
        tris.AddRange(new int[] {c1 - 3, c1 - 2, c5 - 2}); // circular top
    }

    private void UpdateUnityMesh() {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        Vector3[] normals = mesh.normals;
        // foreach (int normal in upNormals) {
        //     normals[normal] = (normals[normal] + Vector3.up * 2).normalized;
        // }
        for (int i = 0; i < 6; i++) {
            List<int> sideNormal = sideNormals[i];
            foreach (int normal in sideNormal) {
                normals[normal] = Rot(i + .5f) * Vector3.forward;
            }
        }
        mesh.normals = normals;
        mesh.RecalculateTangents();
        GetComponent<MeshCollider>().sharedMesh = null;
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }
}
