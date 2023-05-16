using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Land))]
public class Cave : MonoBehaviour {
    public GameObject island;
    public GameObject lava;
    public GameObject floor; // active
    public GameObject secretDoor; // active
    public GameObject publicDoor;
    public GameObject collectibleSpawnUpright;
    public GameObject collectibleSpawnFlipped; // active
    public float islandToPillar = 1.1f;

    void Start() {
        bool isUpright = Vector3.Dot(transform.up, Vector3.up) > 0;
        int version = GetComponent<Land>().RandomRange(4);
        if (isUpright) {
            if (version == 1) {
                floor.SetActive(false);
                secretDoor.SetActive(false);
            } else if (version == 2) {
                lava.SetActive(true);
            } else if (version == 3) {
                lava.SetActive(true);
                island.SetActive(true);
                collectibleSpawnUpright.SetActive(true);
            }
        } else {
            if (version == 1) {
                floor.SetActive(false);
                secretDoor.SetActive(false);
            } else if (version == 2) {
                floor.SetActive(false);
                secretDoor.SetActive(false);
                publicDoor.SetActive(true);
            } else if (version == 3) {
                island.SetActive(true);
                island.transform.localPosition += islandToPillar * Vector3.up;
                island.transform.localScale = new Vector3(1, 2, 1);
                collectibleSpawnFlipped.SetActive(false);
            }
        }
    }
}
