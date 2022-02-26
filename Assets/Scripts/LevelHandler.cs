using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelHandler : MonoBehaviour
{
    public Transform spawnPoint;
    public int minY = -10;
    public int breakParticleDensity = 10;
    public GameObject breakParticlePrefab;
    public int outlineParticleDensity = 10;
    public GameObject outlineParticlePrefab;
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
                foreach (GameObject prefab in new GameObject[]{breakParticlePrefab, outlineParticlePrefab}) {
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
        EndOutlineParticles(o);
        o.GetComponent<SpriteRenderer>().enabled = true;
        foreach (BoxCollider2D collider in o.GetComponents<BoxCollider2D>()) collider.enabled = true;
        if (o.GetComponent<MovingPlatform>() != null) player.transform.parent = null;
        BeginGrowing(o);
    }
    void Disable(GameObject o) {
        o.GetComponent<SpriteRenderer>().enabled = false;
        foreach (BoxCollider2D collider in o.GetComponents<BoxCollider2D>()) collider.enabled = false;
        if (o.GetComponent<MovingPlatform>() != null) {
            player.transform.parent = null;
        }
        SpawnDisableParticles(o);
        StartOutlineParticles(o);
    }

    void SpawnDisableParticles(GameObject o) {
        ParticleSystem particles = o.transform.Find(breakParticlePrefab.name).gameObject.GetComponent<ParticleSystem>();
        var main = particles.main;
        var emission = particles.emission;
        main.startColor = Util.InvertColor(currentColor.color); // hacky solution. might cause issues if there are more than just black/white
        var burst = emission.GetBurst(0);
        burst.count = o.transform.lossyScale.x * o.transform.lossyScale.y * breakParticleDensity;
        emission.SetBurst(0, burst);
        particles.Play();
    }

    void StartOutlineParticles(GameObject o) {
        ParticleSystem particles = o.transform.Find(outlineParticlePrefab.name).gameObject.GetComponent<ParticleSystem>();
        var main = particles.main;
        var emission = particles.emission;
        emission.rateOverTime = outlineParticleDensity * ((transform.lossyScale.x * 2) + (transform.lossyScale.y * 2));
        main.startColor = Util.InvertColor(currentColor.color); // hacky solution. might cause issues if there are more than just black/white
        particles.Play();
    }

    void EndOutlineParticles(GameObject o) {
        ParticleSystem particles = o.transform.Find(outlineParticlePrefab.name).gameObject.GetComponent<ParticleSystem>();
        particles.Stop();
    }

    public void ChangeColor(LevelColor color) {
        foreach (GameObject o in currentColor.objects) Disable(o);
        currentColor = color;
        foreach (GameObject o in currentColor.objects) Enable(o);
    }
}
