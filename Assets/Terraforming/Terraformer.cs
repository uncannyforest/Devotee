using UnityEngine;

public class Terraformer : MonoBehaviour {
    new public Transform camera;

    private HexPos pos = new HexPos();
    private Terrain terrain;

    void Start() {
        terrain = GameObject.FindObjectOfType<Terrain>();
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
