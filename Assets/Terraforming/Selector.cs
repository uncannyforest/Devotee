using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : MonoBehaviour {
    private HexPos pos = new HexPos();

    public HexPos Position {
        get => pos;
        set {
            pos = value;
            if (Terrain.Grid[value]) {
                Vector3 terrainPosition = Terrain.Grid[value].GetChild(0).position;
                transform.position = new Vector3(terrainPosition.x, Mathf.Max(0, terrainPosition.y), terrainPosition.z);
            } else {
                transform.localPosition = pos;
            }
        }
    }

    public Color Color {
        set {
            foreach (Transform child in transform) {
                child.GetComponent<MeshRenderer>().material.SetColor(Shader.PropertyToID("_EmissionColor"), value);
            }
        }
    }
}
