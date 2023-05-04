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
        foreach (Transform child in column.transform) {
            MeshRenderer[] meshes = child.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer mesh in meshes) {
                mesh.material = material;
            }
        }
        return true;
    }
}
