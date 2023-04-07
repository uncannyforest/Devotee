using System;
using System.Collections.Generic;
using UnityEngine;

public class Column : MonoBehaviour {
    public HexPos position;

    public static Column Instantiate(HexPos position, float height) {
        Column column = GameObject.Instantiate<Column>(
                Terrain.I.columnPrefab,
                Quaternion.Euler(0, -30, 0) * (Vector3)(position) * Terrain.I.scale + Vector3.up * height,
                Quaternion.identity,
                Terrain.I.transform
            );
        column.position = position;
        column.gameObject.name = position.ToString();
        Terrain.Grid[position] = column;
        return column;
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

    public Transform InstantiateSurface(SurfaceData data) =>
        InstantiateSurface(data.surfaceId, data.position, data.yRot, data.height, data.zOrientation);

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

    public Transform Surface { get => transform.GetChild(0); }
    public Transform DeepestUnderground { get => transform.GetChild(transform.childCount - 1); }
    public bool HasUnderground { get => transform.childCount > 1; }

    [Serializable] public struct SurfaceData {
        public int surfaceId;
        public Vector3 position;
        public float yRot;
        public int height;
        public int zOrientation;

        public SurfaceData(int surfaceId, Vector3 position, float yRot, int height, int zOrientation) {
            this.surfaceId = surfaceId;
            this.position = position;
            this.yRot = yRot;
            this.height = height;
            this.zOrientation = zOrientation;
        }

        public static SurfaceData From(Transform t) {
            Land land = t.GetComponent<Land>();
            return new SurfaceData(land.id, t.position, t.eulerAngles.y,
                Mathf.RoundToInt(t.lossyScale.y), t.localScale.z > 0 ? 1 : -1);
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
        public SurfaceData surface;
        public UndergroundData[] underground;

        public Data(HexPos position, SurfaceData surface, UndergroundData[] underground) {
            this.position = position;
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
            SurfaceData.From(Surface),
            underground.ToArray()
        );
    }
}
