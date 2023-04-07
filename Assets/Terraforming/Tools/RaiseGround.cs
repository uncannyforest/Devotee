using UnityEngine;

public class RaiseGround : Tool {
    public static int RandomSign() {
        return Random.Range(0, 2) * 2 - 1;
    }

    override public bool Use() => Use(Position);

    public static bool Use(HexPos position) {
        if (!Terrain.I.CanModTerrain(position)) return false;
        if (Terrain.Grid[position] == null) {
            int height = Random.Range(1, Terrain.I.scale + 1);
            Column column = Column.Instantiate(position, -height * 2);
            Transform surface = column.InstantiateSurface(
                Random.Range(0, Terrain.I.randomSurface.Length),
                column.transform.position,
                60 * Random.Range(0, 6),
                height,
                RandomSign());
        } else {
            int yPos = RaiseColumn(position, 1, null);
            MaybeRaiseOrigin(yPos);
        }
        return true;
    }

    public static void MaybeRaiseOrigin(int yPos) {
        if (yPos > Terrain.I.maxHeight) {
            for (int i = 0; i < Terrain.I.originPositions.Length; i++) {
                HexPos origin = Terrain.I.originPositions[i];
                RaiseColumnDangerous(origin, yPos - Terrain.I.maxHeight, i);
            }
            Terrain.I.maxHeight = yPos;
        }
    }

    public static int RaiseColumn(HexPos Position, int quantity, int? groundId) {
        if (!Terrain.I.CanModTerrain(Position)) return 0;
        return RaiseColumnDangerous(Position, quantity, groundId);
    }

    public static int RaiseColumnDangerous(HexPos Position, int quantity, int? groundId) {
        Column column = Terrain.Grid[Position];
        ExtendColumn(Position, -quantity, groundId);
        column.GetComponent<Rigidbody>().MovePosition(column.transform.position + quantity * Vector3Int.up);
        int yPos = Mathf.RoundToInt(column.Surface.position.y) + quantity;
        return yPos;
    }

    public static void ExtendColumn(HexPos Position, int level, int? groundId) {
        Column column = Terrain.Grid[Position];
        while (column.DeepestUnderground.position.y > level) {
            float yPosition = column.DeepestUnderground.position.y - Terrain.I.scale;
            if (column.HasUnderground) yPosition -= Terrain.I.scale;
            int defGroundId = groundId ?? Random.Range(0, Terrain.I.randomUnderground.Length);
            Transform underground = column.InstantiateUnderground(
                defGroundId,
                Quaternion.Euler(0, -30, 0) * (Vector3)(Position) * Terrain.I.scale + yPosition * Vector3.up,
                60 * Random.Range(0, 6),
                180 * Random.Range(0, 2),
                RandomSign());
        }
    }
}
