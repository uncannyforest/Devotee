using UnityEngine;

public class ExplodeGround : Tool {
    private int RandomSign() {
        return Random.Range(0, 2) * 2 - 1;
    }

    override public bool Use() {
        if (!Terrain.I.CanModTerrain(Position)) return false;
        if (Terrain.Grid[Position] == null) return false;
        Transform column = Terrain.Grid[Position];
        if (column.GetChild(0).position.y <= -Terrain.I.scale) return false;

        Vector3 initialPosition = column.GetChild(0).position - Terrain.I.scale * 2 * Vector3.up;
        if (column.childCount >= 2) GameObject.Destroy(column.GetChild(1).gameObject);
        GameObject.Destroy(column.GetChild(0).gameObject);

        int height = Random.Range(1, Terrain.I.scale + 1);
        Transform surface = GameObject.Instantiate(
            Terrain.I.randomSurface[Random.Range(0, Terrain.I.randomSurface.Length)],
            initialPosition,
            Quaternion.Euler(0, 60 * Random.Range(0, 6), 0),
            column).transform;
        surface.localScale = new Vector3(Terrain.I.scale, height, Terrain.I.scale * RandomSign());
        surface.SetAsFirstSibling();

        return true;
    }
}
