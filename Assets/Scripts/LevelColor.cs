using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable()]
public class LevelColor {
    public ColorScheme scheme;
    public GameObject parentObject;
    public GameObject[] objects;
    public LevelColor nextColor;
}
