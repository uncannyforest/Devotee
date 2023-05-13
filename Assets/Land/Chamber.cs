using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Land))]
public class Chamber : MonoBehaviour {
    public GameObject uprightOnly;
    public GameObject flippedOnly;
    public GameObject uprightOnlySometimes;
    public GameObject flippedOnlySometimes;
    public GameObject[] uprightOnlyAlsoHideOne;
    public GameObject[] flippedOnlyAlsoHideOne;

    void Start() {
        bool isUpright = Vector3.Dot(transform.up, Vector3.up) > 0;
        if (isUpright) {
            flippedOnly.SetActive(false);
            flippedOnlySometimes.SetActive(false);
            if (GetComponent<Land>().RandomRange(2) == 0) {
                uprightOnlySometimes.SetActive(false);
                uprightOnlyAlsoHideOne[Random.Range(0, uprightOnlyAlsoHideOne.Length)].SetActive(false);
            }
        } else {
            uprightOnly.SetActive(false);
            uprightOnlySometimes.SetActive(false);
            if (GetComponent<Land>().RandomRange(2) == 0) {
                flippedOnlySometimes.SetActive(false);
                flippedOnlyAlsoHideOne[Random.Range(0, flippedOnlyAlsoHideOne.Length)].SetActive(false);
            }
        }
    }
}
