using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelHandler : MonoBehaviour
{
    public Transform spawnPoint;
    public int minY = -10;
    public int particleDensity = 10;
    public GameObject breakParticlePrefab;
    // Yes, we could just use a dictionary but unity doesn't like dictionaries
    public LevelColor[] level;
    public LevelColor currentColor;
    public float bgColorLerpSpeed = 0.3f;
    public float growSpeed = 2f;

    private Dictionary<GameObject, Vector3> growing = new Dictionary<GameObject, Vector3>();

    private GameObject player;

    void Awake() {
        player = FindObjectOfType<PlayerController>().gameObject;
        foreach (LevelColor lc in level) {
            foreach (GameObject o in lc.objects) {
                
                GameObject particleObject = Instantiate(breakParticlePrefab);
                particleObject.name = breakParticlePrefab.name; // avoid the "(clone)" part
                particleObject.transform.parent = o.transform;
                particleObject.transform.localScale = new Vector3(1, 1, 1);
                particleObject.transform.localPosition = new Vector3(0, 0, 0);
                
                o.GetComponent<SpriteRenderer>().color = lc.color;
                Disable(o);
            }
        }
        ChangeColor(level[0]);
    }

    void Update() {
        Camera.main.backgroundColor = Color.Lerp(Camera.main.backgroundColor, Util.InvertColor(currentColor.color), bgColorLerpSpeed);
        Grow();
    }

    void Grow() {
        // New growing system seems dumb but we cant modify growing while iterating over itad
        Dictionary<GameObject, Vector3> newGrowing = new Dictionary<GameObject, Vector3>();
        foreach (GameObject o in growing.Keys) {
            if (o.GetComponent<MovingPlatform>() != null) {player.transform.parent=null;} // otherwise the player might grow too lol
            o.transform.localScale = Vector3.Lerp(o.transform.localScale, growing[o], Time.deltaTime * growSpeed);
            if (o.transform.localScale.magnitude < growing[o].magnitude - 0.5f) newGrowing[o] = growing[o];
            else {o.transform.localScale = growing[o];}
        }
        growing = newGrowing;
    }

    void BeginGrowing(GameObject o) {
        try {o.transform.localScale = growing[o];}
        catch (KeyNotFoundException) {}
        growing[o] = o.transform.localScale;
        o.transform.localScale = Vector3.zero;
    }

    void Enable(GameObject o) {
        o.GetComponent<SpriteRenderer>().enabled = true;
        o.GetComponent<BoxCollider2D>().enabled = true;
        if (o.GetComponent<MovingPlatform>() != null) {
            player.transform.parent = null;
            o.SetActive(true);
        }
        BeginGrowing(o);
    }
    void Disable(GameObject o) {
        o.GetComponent<SpriteRenderer>().enabled = false;
        o.GetComponent<BoxCollider2D>().enabled = false;
        ParticleSystem particles = o.transform.Find(breakParticlePrefab.name).gameObject.GetComponent<ParticleSystem>();
        var main = particles.main;
        var emission = particles.emission;
        main.startColor = Util.InvertColor(currentColor.color); // hacky solution. might cause issues if there are more than just black/white
        var burst = emission.GetBurst(0);
        burst.count = o.transform.lossyScale.x * o.transform.lossyScale.y * particleDensity;
        emission.SetBurst(0, burst);
        particles.Play();
        if (o.GetComponent<MovingPlatform>() != null) {
            player.transform.parent = null;
            o.SetActive(false);
        }
    }

    public void ChangeColor(LevelColor color) {
        foreach (GameObject o in currentColor.objects) Disable(o);
        currentColor = color;
        foreach (GameObject o in currentColor.objects) Enable(o);
    }
}
