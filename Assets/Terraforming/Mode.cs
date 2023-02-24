using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class Mode : MonoBehaviour {
    public ThirdPersonUserControl character;
    public Terraformer terraformer;
    public GameObject characterCam;
    public GameObject terraformerCam;

    public bool godMode;

    protected void Update() {
        if (Input.GetButtonDown("Mode")) {
            if (godMode) {
                godMode = false;
                characterCam.SetActive(true);
                terraformerCam.SetActive(false);
                character.enabled = true;
                terraformer.enabled = false;
            } else {
                godMode = true;
                characterCam.SetActive(false);
                terraformerCam.SetActive(true);
                character.enabled = false;
                terraformer.enabled = true;
            }
        }
    }

}
