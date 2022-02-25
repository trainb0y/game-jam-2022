using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelHandler : MonoBehaviour
{
    public GameObject breakParticlePrefab;
    public GameObject createParticlePrefab;
    // Yes, we could just use a dictionary but unity doesn't like dictionaries
    public LevelColor[] level;
    public LevelColor currentColor;
    public float bgColorLerpSpeed = 0.3f;

    void Awake() {
    
        foreach (LevelColor lc in level) {
            foreach (GameObject o in lc.objects) {
                foreach (GameObject prefab in  new GameObject[]{createParticlePrefab,  breakParticlePrefab}) {
                    GameObject particleObject = Instantiate(prefab);
                    particleObject.name = prefab.name; // avoid the "(clone)" part
                    particleObject.transform.parent = o.transform;
                    particleObject.transform.localScale = new Vector3(1, 1, 1);
                    particleObject.transform.localPosition = new Vector3(0, 0, 0);
                }
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
        ParticleSystem particles = o.transform.Find(createParticlePrefab.name).gameObject.GetComponent<ParticleSystem>();
        var main = particles.main;
        main.startColor = currentColor.color;
        particles.Play();
    }
    void Disable(GameObject o) {
        o.GetComponent<SpriteRenderer>().enabled = false;
        o.GetComponent<BoxCollider2D>().enabled = false;
        ParticleSystem particles = o.transform.Find(breakParticlePrefab.name).gameObject.GetComponent<ParticleSystem>();
        var main = particles.main;
        main.startColor = Util.InvertColor(currentColor.color); // hacky solution. might cause issues if there are more than just black/white
        particles.Play();
    }

    public void ChangeColor(LevelColor color) {
        foreach (GameObject o in currentColor.objects) Disable(o);
        currentColor = color;
        foreach (GameObject o in currentColor.objects) Enable(o);
    }
}
