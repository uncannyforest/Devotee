using UnityEngine;

public class Terraformer : MonoBehaviour {
    public Interaction interaction;
    new public Transform camera;
    public Material selectorMaterial;
    public Color selectorReady;
    public Color selectorInvalid;

    private HexPos pos = new HexPos();
    private int selectorMaterialColorProperty;

    void Start() {
        Position = new HexPos(2, 0);
        selectorMaterialColorProperty = Shader.PropertyToID("_EmissionColor");
    }

    private HexPos Position {
        get => pos;
        set {
            pos = value;
            if (Terrain.Grid[value]) {
                Vector3 terrainPosition = Terrain.Grid[value].GetChild(0).position;
                transform.position = new Vector3(terrainPosition.x, Mathf.Max(0, terrainPosition.y), terrainPosition.z);
            } else {
                transform.localPosition = pos;
            }
            if (Terrain.I.CanModTerrain(value))
                selectorMaterial.SetColor(selectorMaterialColorProperty, selectorReady);
            else
                selectorMaterial.SetColor(selectorMaterialColorProperty, selectorInvalid);
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
            interaction.Use(Position);
            Position = Position;
        }
    }
}
