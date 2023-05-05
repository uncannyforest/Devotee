using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMaterial : Tool {
    public Material material;

    override public void UpdatePos(HexPos pos) {
        selector.Position = pos;
        if (Terrain.Grid[pos] != null)
            selector.Color = terraformer.selectorReady;
        else
            selector.Color = terraformer.selectorInvalid;
    }

    override public bool Use() {
        Column column = Terrain.Grid[selector.Position];
        if (column == null) return false;
        foreach (Land land in column.GetComponentsInChildren<Land>())
            OnLand(land, material);
        return true;
    }

    public static void OnLand(Land land, Material material) {
        MeshRenderer[] meshes = land.ground;
        foreach (MeshRenderer mesh in meshes) {
            mesh.material = material;
        }
    }

    public static Material GetAdjacent(HexPos pos) {
        List<Column> adjCols = (new Column[] { Terrain.Grid[pos + HexPos.E], Terrain.Grid[pos + HexPos.W], Terrain.Grid[pos + HexPos.Q],
                Terrain.Grid[pos + HexPos.A], Terrain.Grid[pos + HexPos.S], Terrain.Grid[pos + HexPos.D] })
            .Where(c => c != null)
            .ToList();
        if (adjCols.Count == 0) return Terrain.I.defaultMaterial;
        return adjCols[Random.Range(0, adjCols.Count)].Material;
    }
}
