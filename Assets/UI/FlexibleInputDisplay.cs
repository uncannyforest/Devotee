using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlexibleInputDisplay : MonoBehaviour {
    public GameObject interactNotice1;

    public void UpdateForHeldObject(GameObject heldObject) {
        SetInteractionMessage("release");
    }

    public void SetInteractionMessage(string interact1) {
        SetInteractionMessage(interactNotice1, interact1, "x");
    }

    private void SetInteractionMessage(GameObject interactNotice, string message, string desktopKey) {
        if (!string.IsNullOrEmpty(message)) {
            interactNotice.SetActive(true);

        message = "press <color=white>" + desktopKey + "</color> to <color=white>"
            + message + "</color>";


            interactNotice.transform.Find("Text").gameObject.GetComponent<Text>().text = message;
        } else {
            interactNotice.SetActive(false);
        }

    }
}
