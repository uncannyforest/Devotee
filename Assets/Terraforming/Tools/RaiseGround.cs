using UnityEngine;

public class RaiseGround : Tool {

    override public bool Use() => Use(Position);

    public static bool Use(HexPos position) {
        if (!Terrain.I.CanModTerrain(position)) return false;
        if (Terrain.Grid[position] == null) {
            Column column = Column.Instantiate(position, 0, SurfaceHeight.RandomSurface());
        } else {
            SurfaceHeight.RaiseColumn(position, 1, null);
        }
        return true;
    }
}
