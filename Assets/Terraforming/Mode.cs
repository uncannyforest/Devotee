using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class Mode : MonoBehaviour {
    public ThirdPersonUserControl character;
    public Terraformer terraformer;
    public GameObject characterCam;
    public GameObject terraformerCam;
    public GameObject characterLight;
    public GameObject terraformerLight;
    public Camera uiCam;

    public bool godMode;

    void Update() {
        if (Input.GetButtonDown("Mode")) {
            if (godMode) {
                godMode = false;
                characterCam.SetActive(true);
                terraformerCam.SetActive(false);
                characterLight.SetActive(true);
                terraformerLight.SetActive(false);
                character.AllowMove("mode");
                terraformer.enabled = false;
                uiCam.cullingMask = LayerMask.GetMask("UI", "Perspective UI Only");
            } else {
                godMode = true;
                characterCam.SetActive(false);
                terraformerCam.SetActive(true);
                characterLight.SetActive(false);
                terraformerLight.SetActive(true);
                character.ProhibitMove("mode");
                terraformer.enabled = true;
                uiCam.cullingMask = LayerMask.GetMask("UI", "Orthographic UI Only");
            }
        }
    }

}
