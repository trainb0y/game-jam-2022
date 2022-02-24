using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelHandler : MonoBehaviour
{
    // Yes, we could just use a dictionary but unity 
    public LevelColor[] level;
    public LevelColor currentColor;
    public float bgColorLerpSpeed = 0.3f;

    void Awake() {
        foreach (LevelColor lc in level) foreach (GameObject o in lc.objects) o.SetActive(false);
        ChangeColor(level[0]);
    }

    void Update() {
        Camera.main.backgroundColor = Color.Lerp(Camera.main.backgroundColor, Util.InvertColor(currentColor.color), bgColorLerpSpeed);
    }

    public void ChangeColor(LevelColor color) {
        foreach (GameObject o in currentColor.objects) o.SetActive(false);
        currentColor = color;
        foreach (GameObject o in currentColor.objects) {
            o.SetActive(true);
            o.GetComponent<SpriteRenderer>().color = color.color;
        }
    }
}
