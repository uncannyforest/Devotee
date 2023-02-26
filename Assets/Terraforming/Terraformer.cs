using UnityEngine;

public class Terraformer : MonoBehaviour {
    new public Transform camera;
    public Material selectorMaterial;
    public Color selectorReady;
    public Color selectorInvalid;

    private HexPos pos = new HexPos();
    private Terrain terrain;
    private int selectorMaterialColorProperty;

    void Start() {
        terrain = GameObject.FindObjectOfType<Terrain>();
        Position = new HexPos(2, 0);
        selectorMaterialColorProperty = Shader.PropertyToID("_EmissionColor");
    }

    private HexPos Position {
        get => pos;
        set {
            pos = value;
            if (terrain.grid[value]) {
                Vector3 terrainPosition = terrain.grid[value].position;
                transform.position = new Vector3(terrainPosition.x, Mathf.Max(0, terrainPosition.y), terrainPosition.z);
            } else {
                transform.localPosition = pos;
            }
            if (terrain.CanModTerrain(value))
                selectorMaterial.SetColor(selectorMaterialColorProperty, selectorReady);
            else
                selectorMaterial.SetColor(selectorMaterialColorProperty, selectorInvalid);
        }
    }

    // Update is called once per frame
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
            terrain.RaiseGround(Position);
            Position = Position;
        }
    }
}
