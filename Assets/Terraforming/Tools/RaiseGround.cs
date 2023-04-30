using UnityEngine;

public class RaiseGround : Tool {

    override public bool Use() => Use(Position);

    public override void UpdatePos(HexPos pos) {
        selector.Position = pos;
        if (Terrain.I.CanRaiseTerrain(pos)) selector.Color = terraformer.selectorReady;
        else selector.Color = terraformer.selectorInvalid;
    }

    public static bool Use(HexPos position) {
        if (!Terrain.I.CanRaiseTerrain(position)) return false;
        if (Terrain.Grid[position] == null) {
            Column column = Column.Instantiate(position, 0, SurfaceHeight.RandomSurface());
        } else {
            SurfaceHeight.RaiseColumn(position, 1, null);
        }
        return true;
    }
}
