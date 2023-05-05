using UnityEngine;

public class ExplodeGround : Tool {
    private int RandomSign() {
        return Random.Range(0, 2) * 2 - 1;
    }

    override public bool Use() {
        if (!Terrain.I.CanModTerrain(Position)) return false;
        if (Terrain.Grid[Position] == null) return false;
        Column column = Terrain.Grid[Position];
        if (column.Surface.position.y <= -Terrain.Scale) return false;

        Vector3 initialPosition = column.Surface.position - Terrain.Scale * 2 * Vector3.up;
        if (column.transform.childCount >= 2) GameObject.Destroy(column.transform.GetChild(1).gameObject);
        GameObject.Destroy(column.Surface.gameObject);

        Transform surface = column.InstantiateSurface(Position, initialPosition, SurfaceHeight.RandomSurface());
        surface.SetAsFirstSibling();

        return true;
    }
}
