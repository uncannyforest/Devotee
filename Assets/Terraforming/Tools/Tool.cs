using System;
using UnityEngine;

public abstract class Tool : MonoBehaviour {
    protected Selector selector;
    protected HexPos Position {
        get => selector.Position;
    }

    void Start() {
        selector = GameObject.FindObjectOfType<Selector>();
    }

    public virtual void WillUpdatePos(HexPos pos) {}
    public abstract bool Use();
    public virtual void Load() {}
    public virtual void Unload() {}
}
