using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terrain : MonoBehaviour {
    public int scale = 10;
    public GameObject columnPrefab;
    public GameObject[] randomSurface;
    public GameObject[] randomUnderground;

    public HexGrid<Transform> grid = new HexGrid<Transform>();

    private int RandomSign() {
        return Random.Range(0, 2) * 2 - 1;
    }

    public void RaiseGround(HexPos pos) {
        if (grid[pos] == null) {
            int height = Random.Range(1, scale + 1);
            Vector3 initialPosition = Quaternion.Euler(0, -30, 0) * (Vector3)(pos) * scale + Vector3.down * Mathf.Floor(height * 1.5f);
            Transform column = GameObject.Instantiate(
                columnPrefab,
                initialPosition,
                Quaternion.identity,
                transform
            ).transform;
            column.gameObject.name = pos.ToString();
            Transform surface = GameObject.Instantiate(
                randomSurface[Random.Range(0, randomSurface.Length)],
                initialPosition,
                Quaternion.Euler(0, 60 * Random.Range(0, 6), 0),
                column).transform;
            surface.localScale = new Vector3(scale, height, scale * RandomSign());
            grid[pos] = column;
        } else {
            Transform column = grid[pos];
            if (column.GetChild(column.childCount - 1).position.y > -1) {
                float yPosition = column.GetChild(column.childCount - 1).position.y - scale;
                if (column.childCount > 1) yPosition -= scale;
                Transform underground = GameObject.Instantiate(
                    randomUnderground[Random.Range(0, randomUnderground.Length)],
                    Quaternion.Euler(0, -30, 0) * (Vector3)(pos) * scale + yPosition * Vector3.up,
                    Quaternion.Euler(0, 60 * Random.Range(0, 6), 180 * Random.Range(0, 2)),
                    column).transform;
                underground.localScale = new Vector3(scale, scale, scale * RandomSign());
            }
            column.GetComponent<Rigidbody>().MovePosition(column.position + Vector3Int.up);
        }
    }
}
