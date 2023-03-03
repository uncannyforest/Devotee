using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Terrain : MonoBehaviour {
    private static Terrain instance;
    public static Terrain I { get => instance; }
    Terrain(): base() {
        instance = this;
    }

    public int scale = 10;
    public HexPos[] originPositions = new HexPos[] {
        new HexPos(0, 0),
        new HexPos(1, 0)
    };
    public GameObject columnPrefab;
    public GameObject[] randomSurface;
    public GameObject[] randomUnderground;

    public HexGrid<Transform> grid = new HexGrid<Transform>();
    public static HexGrid<Transform> Grid { get => instance.grid; }

    public void Start() {
        grid[new HexPos(0, 0)] = transform.GetChild(0);
        grid[new HexPos(1, 0)] = transform.GetChild(1);
    }

    public bool CanModTerrain(HexPos pos) {
        return !originPositions.Contains(pos);
    }
}
