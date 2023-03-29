using UnityEngine;

public class Terraformer : MonoBehaviour {
    public Interaction interaction;
    new public Transform camera;
    public Selector selector;
    public Color selectorReady;
    public Color selectorInvalid;

    void Start() {
        Position = new HexPos(2, 0);
    }

    public HexPos Position {
        get => selector.Position;
        private set {
            interaction.WillUpdatePos(value);
            selector.Position = value;
            if (Terrain.I.CanModTerrain(value))
                selector.Color = selectorReady;
            else
                selector.Color = selectorInvalid;
        }
    }

    void Update() {
        if (Input.GetKeyDown("e")) {
            Position += HexPos.E.rotate(-camera.eulerAngles.y - 30);
        }
        if (Input.GetKeyDown("w")) {
            Position += HexPos.W.rotate(-camera.eulerAngles.y - 30);
        }
        if (Input.GetKeyDown("q")) {
            Position += HexPos.Q.rotate(-camera.eulerAngles.y - 30);
        }
        if (Input.GetKeyDown("a")) {
            Position += HexPos.A.rotate(-camera.eulerAngles.y - 30);
        }
        if (Input.GetKeyDown("s")) {
            Position += HexPos.S.rotate(-camera.eulerAngles.y - 30);
        }
        if (Input.GetKeyDown("d")) {
            Position += HexPos.D.rotate(-camera.eulerAngles.y - 30);
        }
        if (Input.GetKeyDown("space")) {
            interaction.Use();
            Position = Position;
        }
    }
}
