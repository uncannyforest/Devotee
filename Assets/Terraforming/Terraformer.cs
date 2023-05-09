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
            interaction.UpdatePos(value);
        }
    }

    void Update() {
        if (Input.GetKeyDown("e")) {
            Position += HexPos.E.Rotate(-camera.eulerAngles.y - 30);
        }
        if (Input.GetKeyDown("w")) {
            Position += HexPos.W.Rotate(-camera.eulerAngles.y - 30);
        }
        if (Input.GetKeyDown("q")) {
            Position += HexPos.Q.Rotate(-camera.eulerAngles.y - 30);
        }
        if (Input.GetKeyDown("a")) {
            Position += HexPos.A.Rotate(-camera.eulerAngles.y - 30);
        }
        if (Input.GetKeyDown("s")) {
            Position += HexPos.S.Rotate(-camera.eulerAngles.y - 30);
        }
        if (Input.GetKeyDown("d")) {
            Position += HexPos.D.Rotate(-camera.eulerAngles.y - 30);
        }
        if (Input.GetKeyDown("space")) {
            interaction.Select();
            Position = Position;
        }
    }
}
