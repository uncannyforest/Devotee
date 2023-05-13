using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TriPos {
    public HexPos hexPos;
    public bool top;

    public TriPos(HexPos hexPos, bool top) {
        this.hexPos = hexPos;
        this.top = top;
    }

    public HexPos[] Sides {
        get => top ? new HexPos[] {
            hexPos,
            hexPos + HexPos.E,
            hexPos + HexPos.W
        } : new HexPos[] {
            hexPos,
            hexPos + HexPos.A,
            hexPos + HexPos.S
        };
    }

    public int[] SideCorners {
        get => top ? new int[] {1, 3, 5} : new int[] {4, 0, 2};
    }

    override public string ToString() => "(" + hexPos.x + ", " + hexPos.y + ", " + (top ? 1 : 0) + ")";
}

public class CircularIntersectionManager : MonoBehaviour {
    private static CircularIntersectionManager instance;
    public static CircularIntersectionManager I { get => instance; }
    CircularIntersectionManager(): base() {
        instance = this;
    }

    public Transform universalParent;
    public GameObject prefab;
    public int oceanFloor = -1;

    private HexGrid<Transform[]> intersections = new HexGrid<Transform[]>();

    public void Awake() {
        intersections[HexPos.E] = new Transform[] {universalParent.GetChild(0), null};
        intersections[HexPos.S] = new Transform[] {null, universalParent.GetChild(1)};
    }

    private Transform GetIntersection(TriPos triangle) {
        Transform[] pair = intersections[triangle.hexPos];
        if (pair == null) return null;
        return pair[triangle.top ? 1 : 0];
    }

    private void RemoveIntersection(TriPos triangle) {
        Transform[] pair = intersections[triangle.hexPos];
        if (pair == null) return;
        Transform t = pair[triangle.top ? 1 : 0];
        if (t == null) return;
        GameObject.Destroy(t.gameObject);
        pair[triangle.top ? 1 : 0] = null;
    }

    private Transform GetOrMakeIntersection(TriPos triangle) {
        Transform[] pair = intersections[triangle.hexPos];
        if (pair == null) {
            pair = new Transform[] {null, null};
            intersections[triangle.hexPos] = pair;
        } else if (pair[triangle.top ? 1 : 0] != null) {
            return pair[triangle.top ? 1 : 0];
        }
        GameObject intersection = GameObject.Instantiate(prefab, triangle.hexPos.World + oceanFloor * Vector3.up,
                triangle.top ? Quaternion.Euler(0, 180,0): Quaternion.identity, universalParent);
        intersection.name = triangle.ToString();
        pair[triangle.top ? 1 : 0] = intersection.transform;
        return intersection.transform;
    }

    public void UpdateHex(HexPos pos) {
        foreach (TriPos triangle in pos.Corners) UpdateTri(triangle);
    }

    public void UpdateTri(TriPos triangle) {
        int maxHeight = oceanFloor;
        int midHeight = oceanFloor;
        Material midMaterial = null;
        Material material1 = null;
        Material material2 = null;
        HexPos[] sides = triangle.Sides;
        int[] corners = triangle.SideCorners;
        for (int i = 0; i < 3; i++) {
            HexPos pos = sides[i];
            Debug.Log(pos + " " + corners[i] + " " + (Terrain.Grid[pos] != null));
            if (Terrain.Grid[pos] == null) continue;
            int height = SurfaceHeight.GetHeight(pos, corners[i]) ?? oceanFloor;
            if (height > maxHeight) {
                midHeight = maxHeight;
                maxHeight = height;
            } else if (height > midHeight) {
                midHeight = height;
            }
            Material material = Terrain.Grid[pos].Material;
            if (midMaterial == null && material != null) {
                if (material1 == null) {
                    material1 = Terrain.Grid[pos].Material;
                } else if (material1.name == material.name) {
                    midMaterial = material;
                } else if (material2 == null) {
                    material2 = material;
                } else if (material2.name == material.name) {
                    midMaterial = material;
                }
            }
            Debug.Log(height + " " + material.name + " " + maxHeight + " " + midHeight + " " + midMaterial?.name);
        }
        if (midMaterial == null && material2 == null) midMaterial = material1;

        if (midHeight == oceanFloor && GetIntersection(triangle) != null) {
            RemoveIntersection(triangle);
        } else if (midHeight > oceanFloor) {
            Transform intersection = GetOrMakeIntersection(triangle);
            MoveTo(intersection, midHeight);
            if (midMaterial != null) {
                intersection.GetComponent<MeshRenderer>().sharedMaterial = midMaterial;
            }
        }
    }

    public void MoveTo(Transform intersection, int newHeight) {
        intersection.GetComponent<Rigidbody>().MovePosition(new Vector3(intersection.position.x, newHeight, intersection.position.z));
        intersection.localScale = new Vector3(Terrain.Scale, newHeight - oceanFloor, Terrain.Scale);
    }
}
