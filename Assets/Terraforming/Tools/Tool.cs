using System;
using UnityEngine;

public abstract class Tool : MonoBehaviour {
    public abstract bool Use(HexPos pos);
}
