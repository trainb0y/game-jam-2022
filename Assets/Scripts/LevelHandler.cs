using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.ParticleSystem;

public class LevelHandler : MonoBehaviour
{
    public GameObject particlePrefab;
    // Yes, we could just use a dictionary but unity doesn't like dictionaries
    public LevelColor[] level;
    public LevelColor currentColor;
    public float bgColorLerpSpeed = 0.3f;

    void Awake() {
        foreach (LevelColor lc in level) {
            foreach (GameObject o in lc.objects) {
                GameObject particleObject = Instantiate(particlePrefab);
                particleObject.name = particlePrefab.name; // avoid the "(clone)" part
                particleObject.transform.parent = o.transform;
                particleObject.transform.localScale = o.transform.localScale;
                particleObject.transform.localPosition = new Vector3(0, 0, 0);
                var trails = particleObject.GetComponent<ParticleSystem>().trails;
                trails.colorOverLifetime = new ParticleSystem.MinMaxGradient(lc.color, Util.InvertColor(lc.color));
                o.GetComponent<SpriteRenderer>().color = lc.color;
                Disable(o);
            }
        }
        ChangeColor(level[0]);
    }

    void Update() {
        Camera.main.backgroundColor = Color.Lerp(Camera.main.backgroundColor, Util.InvertColor(currentColor.color), bgColorLerpSpeed);
    }

    void Enable(GameObject o) {
        o.GetComponent<SpriteRenderer>().enabled = true;
        o.GetComponent<BoxCollider2D>().enabled = true;
        o.transform.Find(particlePrefab.name).gameObject.SetActive(false);
    }
    void Disable(GameObject o) {
        o.GetComponent<SpriteRenderer>().enabled = false;
        o.GetComponent<BoxCollider2D>().enabled = false;
        o.transform.Find(particlePrefab.name).gameObject.SetActive(true);
    }

    public void ChangeColor(LevelColor color) {
        foreach (GameObject o in currentColor.objects) Disable(o);
        currentColor = color;
        foreach (GameObject o in currentColor.objects) Enable(o);
    }
}
