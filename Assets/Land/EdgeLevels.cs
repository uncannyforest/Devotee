using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeLevels : MonoBehaviour {
    public int[] levels;

    // facing along x axis in world space = up (W)
    public float GetLevel(HexPos relative) {
        HexPos afterRotation = relative.rotate(transform.localRotation.eulerAngles.y);
        HexPos check = transform.localScale.z > 0 ? afterRotation : new HexPos(afterRotation.x + afterRotation.y, -afterRotation.y);
        if (check == HexPos.D) return levels[0] * .25f * transform.lossyScale.y + transform.position.y;
        if (check == HexPos.E) return levels[1] * .25f * transform.lossyScale.y + transform.position.y;
        if (check == HexPos.W) return levels[2] * .25f * transform.lossyScale.y + transform.position.y;
        if (check == HexPos.Q) return levels[3] * .25f * transform.lossyScale.y + transform.position.y;
        if (check == HexPos.A) return levels[4] * .25f * transform.lossyScale.y + transform.position.y;
        if (check == HexPos.S) return levels[5] * .25f * transform.lossyScale.y + transform.position.y;
        throw new ArgumentException("Passed in " + relative);
    }
}
