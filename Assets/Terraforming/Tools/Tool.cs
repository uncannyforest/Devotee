using System;
using UnityEngine;

public abstract class Tool : MonoBehaviour {
    protected Selector selector;
    protected Terraformer terraformer;
    protected HexPos Position {
        get => selector.Position;
    }

    void Start() {
        selector = GameObject.FindObjectOfType<Selector>();
        terraformer = GameObject.FindObjectOfType<Terraformer>();
    }

    public virtual void UpdatePos(HexPos pos) {
        selector.Position = pos;
        if (Terrain.I.CanModTerrain(pos))
            selector.Color = terraformer.selectorReady;
        else
            selector.Color = terraformer.selectorInvalid;
    }
    public abstract bool Use();
    public virtual void Load() {}
    public virtual void Unload() {}
}
