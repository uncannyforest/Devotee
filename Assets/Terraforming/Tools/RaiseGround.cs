using UnityEngine;

public class RaiseGround : Tool {
    private int maxHeight = 0;

    public static int RandomSign() {
        return Random.Range(0, 2) * 2 - 1;
    }

    override public bool Use() => Use(Position, ref maxHeight);

    public static bool Use(HexPos position, ref int maxHeight) {
        if (!Terrain.I.CanModTerrain(position)) return false;
        if (Terrain.Grid[position] == null) {
            int height = Random.Range(1, Terrain.I.scale + 1);
            Vector3 initialPosition = Quaternion.Euler(0, -30, 0) * (Vector3)(position) * Terrain.I.scale + Vector3.down * Mathf.Floor(height * 1.5f);
            Transform column = GameObject.Instantiate(
                Terrain.I.columnPrefab,
                initialPosition,
                Quaternion.identity,
                Terrain.I.transform
            ).transform;
            column.gameObject.name = position.ToString();
            Transform surface = GameObject.Instantiate(
                Terrain.I.randomSurface[Random.Range(0, Terrain.I.randomSurface.Length)],
                initialPosition,
                Quaternion.Euler(0, 60 * Random.Range(0, 6), 0),
                column).transform;
            surface.localScale = new Vector3(Terrain.I.scale, height, Terrain.I.scale * RandomSign());
            Terrain.Grid[position] = column;
            Mathf.RoundToInt(initialPosition.y);
        } else {
            int yPos = RaiseColumn(position, 1, null);
            MaybeRaiseOrigin(yPos, ref maxHeight);
        }
        return true;
    }

    public static void MaybeRaiseOrigin(int yPos, ref int maxHeight) {
        if (yPos > maxHeight) {
            for (int i = 0; i < Terrain.I.originPositions.Length; i++) {
                HexPos origin = Terrain.I.originPositions[i];
                RaiseColumnDangerous(origin, yPos - maxHeight, i);
            }
            maxHeight = yPos;
        }
    }

    public static int RaiseColumn(HexPos Position, int quantity, int? groundId) {
        if (!Terrain.I.CanModTerrain(Position)) return 0;
        return RaiseColumnDangerous(Position, quantity, groundId);
    }

    public static int RaiseColumnDangerous(HexPos Position, int quantity, int? groundId) {
        Transform column = Terrain.Grid[Position];
        ExtendColumn(Position, -quantity, groundId);
        column.GetComponent<Rigidbody>().MovePosition(column.position + quantity * Vector3Int.up);
        int yPos = Mathf.RoundToInt(column.GetChild(0).position.y) + quantity;
        return yPos;
    }

    public static void ExtendColumn(HexPos Position, int level, int? groundId) {
        Transform column = Terrain.Grid[Position];
        while (column.GetChild(column.childCount - 1).position.y > level) {
            float yPosition = column.GetChild(column.childCount - 1).position.y - Terrain.I.scale;
            if (column.childCount > 1) yPosition -= Terrain.I.scale;
            int defGroundId = groundId ?? Random.Range(0, Terrain.I.randomUnderground.Length);
            Transform underground = GameObject.Instantiate(
                Terrain.I.randomUnderground[defGroundId],
                Quaternion.Euler(0, -30, 0) * (Vector3)(Position) * Terrain.I.scale + yPosition * Vector3.up,
                Quaternion.Euler(0, 60 * Random.Range(0, 6), 180 * Random.Range(0, 2)),
                column).transform;
            underground.localScale = new Vector3(Terrain.I.scale, Terrain.I.scale, Terrain.I.scale * RandomSign());
        }
    }
}
