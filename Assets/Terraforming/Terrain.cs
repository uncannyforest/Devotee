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
    public Column columnPrefab;
    public MeshGenerator surfaceMesh;
    public GameObject[] randomUnderground;

    public HexGrid<Column> grid = new HexGrid<Column>();
    public static HexGrid<Column> Grid { get => instance.grid; }
    public static int Scale { get => instance.scale; }

    public int maxHeight = 0;

    private Transform player;

    public void Awake() {
        grid[new HexPos(0, 0)] = transform.GetChild(0).GetComponent<Column>();
        grid[new HexPos(1, 0)] = transform.GetChild(1).GetComponent<Column>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public bool CanRaiseTerrain(HexPos pos) {
        return !originPositions.Contains(pos);
    }

    public bool CanModTerrain(HexPos pos) {
        return CanRaiseTerrain(pos) && HexPos.FromWorld(player.position) != pos;
    }

    public void PopulateTerrainFromData(Column.Data[] land) {
        Vector3 startLoc = Terrain.Grid[originPositions[1]].transform.position;
        foreach (Column.Data column in land) Column.Instantiate(column);
        Transform player = GameObject.FindGameObjectWithTag("Player").transform;
        player.position += Terrain.Grid[originPositions[1]].transform.position - startLoc;
    }
}
