using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Column : MonoBehaviour {
    public HexPos position;
    public string debugInfo;

    private int dyNextUpdate;
    public int Y { get => Mathf.RoundToInt(Surface.transform.position.y) + dyNextUpdate; }

    public static Column Instantiate(Data data) {
        Column column = Terrain.Grid[data.position];

        if (column) {
            column.transform.position += Vector3.up * (data.maxHeight - column.MaxHeight);
            for (int i = column.UndergroundCount; i < data.underground.Length; i++) {
                UndergroundData underground = data.underground[i];
                column.InstantiateUnderground(underground.undergroundId, underground.position,
                    underground.yRot, underground.zRot, underground.zOrientation, underground.randomSeed);
            }
            return column;
        }

        column = Instantiate(data.position, data.maxHeight, data.surface.corners);
        foreach (UndergroundData underground in data.underground) {
            column.InstantiateUnderground(underground.undergroundId, underground.position,
                underground.yRot, underground.zRot, underground.zOrientation, underground.randomSeed);
        }
        return column;
    }

    public static Column Instantiate(HexPos position, int maxHeight, int[] surface) {
        int height = maxHeight - Mathf.Max(surface);
        Column column = GameObject.Instantiate<Column>(
                Terrain.I.columnPrefab,
                Quaternion.Euler(0, -30, 0) * (Vector3)(position) * Terrain.I.scale + Vector3.up * height,
                Quaternion.identity,
                Terrain.I.transform
            );
        column.position = position;
        column.gameObject.name = position.ToString();
        Terrain.Grid[position] = column;
        column.InstantiateSurface(column.transform.position, surface);
        return column;
    }

    public Transform InstantiateSurface(Vector3 position, int[] corners) {
        MeshGenerator surface = GameObject.Instantiate<MeshGenerator>(
            Terrain.I.surfaceMesh,
            position,
            Quaternion.identity,
            transform);
        surface.corners = corners;
        surface.transform.localScale = Vector3.one * Terrain.I.scale;
        return surface.transform;
    }

    public Transform InstantiateSurface
            (int surfaceId, Vector3 position, float yRot, int height, int zOrientation) {
        Transform result = GameObject.Instantiate(
                Terrain.I.randomSurface[surfaceId],
                position,
                Quaternion.Euler(0, yRot, 0),
                transform).transform;
        result.localScale = new Vector3(Terrain.I.scale, height, Terrain.I.scale * zOrientation);
        return result;
    }

    public Transform InstantiateUnderground
            (int undergroundId, Vector3 position, float yRot,
             float zRot, int zOrientation, float? randomSeed) {
        Transform result = GameObject.Instantiate(
                Terrain.I.randomUnderground[undergroundId],
                position,
                Quaternion.Euler(0, yRot, zRot),
                transform).transform;
        result.localScale = new Vector3(Terrain.I.scale, Terrain.I.scale, Terrain.I.scale * zOrientation);
        if (randomSeed is float aRandomSeed) result.GetComponent<Land>().randomSeed = aRandomSeed;
        return result;
    }

    public Transform InstantiateUnderground
            (int undergroundId, Vector3 position, float yRot,
             float zRot, int zOrientation) =>
        InstantiateUnderground(undergroundId, position, yRot,
                zRot, zOrientation, null);

    public Transform InstantiateUnderground(UndergroundData data) =>
        InstantiateUnderground(data.undergroundId, data.position, data.yRot,
            data.zRot, data.zOrientation, data.randomSeed);

    public int[] Heights { get => Surface.GetComponent<MeshGenerator>().Corners.heightsWithBase(Y); }
    public int MaxHeight { get => Mathf.Max(Surface.GetComponent<MeshGenerator>().Corners) + Y; }
    public Transform Surface { get => transform.GetChild(0); }
    public Transform DeepestUnderground { get => transform.GetChild(transform.childCount - 1); }
    public bool HasUnderground { get => transform.childCount > 1; }
    public int UndergroundCount { get => transform.childCount - 1; }

    public int GetHeight(int corner) {
        return Surface.GetComponent<MeshGenerator>().corners[corner] + Y;
    }
    public void SetHeights(int[] heights, bool raiseFloorToClamp) {
        int height = Surface.GetComponent<MeshGenerator>().SetCornersClampReturningDiff(heights, raiseFloorToClamp);
        debugInfo += "SetHeights " + heights[0] + " " + heights[1] + " " + heights[2] + " " + heights[3] + " " + heights[4] + " " + heights[5] + " height " + height + " Y " + Y;
        Move(height - Y);
    }
    public void Move(int diff) {
        if (diff > 0 || dyNextUpdate != 0) {
            SurfaceHeight.RaiseColumn(position, diff, null);
        } else if (diff < 0) {
            GetComponent<Rigidbody>().MovePosition(transform.position + diff * Vector3Int.up);
        }
    }
    public void RigidbodyMove(int quantity) {
        dyNextUpdate += quantity;
        GetComponent<Rigidbody>().MovePosition(transform.position + dyNextUpdate * Vector3Int.up);
    }

    [Serializable] public struct SurfaceData {
        public int[] corners;

        public SurfaceData(int[] corners) {
            this.corners = corners;
        }

        public static SurfaceData From(Transform t) {
            MeshGenerator land = t.GetComponent<MeshGenerator>();
            return new SurfaceData(land.corners);
        }
    }

    [Serializable] public struct UndergroundData {
        public int undergroundId;
        public Vector3 position;
        public float yRot;
        public float zRot;
        public int zOrientation;
        public float randomSeed;

        public UndergroundData(int undergroundId, Vector3 position, float yRot,
                float zRot, int zOrientation, float randomSeed) {
            this.undergroundId = undergroundId;
            this.position = position;
            this.yRot = yRot;
            this.zRot = zRot;
            this.zOrientation = zOrientation;
            this.randomSeed = randomSeed;
        }

        public static UndergroundData From(Transform t) {
            Land land = t.GetComponent<Land>();
            return new UndergroundData(land.id, t.position, t.eulerAngles.y,
                t.eulerAngles.z, t.localScale.z > 0 ? 1 : -1, land.randomSeed);
        }
    }

    [Serializable] public struct Data {
        public HexPos position;
        public int maxHeight;
        public SurfaceData surface;
        public UndergroundData[] underground;

        public Data(HexPos position, int maxHeight, SurfaceData surface, UndergroundData[] underground) {
            this.position = position;
            this.maxHeight = maxHeight;
            this.surface = surface;
            this.underground = underground;
        }
    }

    public Data Serialize() {
        List<UndergroundData> underground = new List<UndergroundData>();
        foreach (Transform child in transform)
            if (child.GetSiblingIndex() != 0)
                underground.Add(UndergroundData.From(child));

        return new Data(
            position,
            MaxHeight,
            SurfaceData.From(Surface),
            underground.ToArray()
        );
    }

    void FixedUpdate() {
        if (dyNextUpdate != 0) dyNextUpdate = 0;
    }
}
