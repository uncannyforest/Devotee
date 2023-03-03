using UnityEngine;

public class RaiseGround : Tool {
    private int maxHeight = 0;

    private int RandomSign() {
        return Random.Range(0, 2) * 2 - 1;
    }

    override public bool Use(HexPos pos) {
        if (!Terrain.I.CanModTerrain(pos)) return false;
        int yPos;
        if (Terrain.Grid[pos] == null) {
            int height = Random.Range(1, Terrain.I.scale + 1);
            Vector3 initialPosition = Quaternion.Euler(0, -30, 0) * (Vector3)(pos) * Terrain.I.scale + Vector3.down * Mathf.Floor(height * 1.5f);
            Transform column = GameObject.Instantiate(
                Terrain.I.columnPrefab,
                initialPosition,
                Quaternion.identity,
                Terrain.I.transform
            ).transform;
            column.gameObject.name = pos.ToString();
            Transform surface = GameObject.Instantiate(
                Terrain.I.randomSurface[Random.Range(0, Terrain.I.randomSurface.Length)],
                initialPosition,
                Quaternion.Euler(0, 60 * Random.Range(0, 6), 0),
                column).transform;
            surface.localScale = new Vector3(Terrain.I.scale, height, Terrain.I.scale * RandomSign());
            Terrain.Grid[pos] = column;
            yPos = Mathf.RoundToInt(initialPosition.y);
        } else {
            Transform column = RaiseColumn(pos, null);
            yPos = Mathf.RoundToInt(column.GetChild(0).position.y) + 1;
        }
        if (yPos > maxHeight) {
            for (int i = 0; i < Terrain.I.originPositions.Length; i++) {
                HexPos origin = Terrain.I.originPositions[i];
                RaiseColumn(origin, i);
            }
            maxHeight = yPos;
        }
        return true;
    }

    private Transform RaiseColumn(HexPos pos, int? groundId) {
        Transform column = Terrain.Grid[pos];
        if (column.GetChild(column.childCount - 1).position.y > -1) {
            float yPosition = column.GetChild(column.childCount - 1).position.y - Terrain.I.scale;
            if (column.childCount > 1) yPosition -= Terrain.I.scale;
            int defGroundId = groundId ?? Random.Range(0, Terrain.I.randomUnderground.Length);
            Transform underground = GameObject.Instantiate(
                Terrain.I.randomUnderground[defGroundId],
                Quaternion.Euler(0, -30, 0) * (Vector3)(pos) * Terrain.I.scale + yPosition * Vector3.up,
                Quaternion.Euler(0, 60 * Random.Range(0, 6), 180 * Random.Range(0, 2)),
                column).transform;
            underground.localScale = new Vector3(Terrain.I.scale, Terrain.I.scale, Terrain.I.scale * RandomSign());
        }
        column.GetComponent<Rigidbody>().MovePosition(column.position + Vector3Int.up);
        return column;
    }
}
