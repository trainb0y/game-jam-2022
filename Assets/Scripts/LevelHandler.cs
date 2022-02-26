using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelHandler : MonoBehaviour
{
    public GameObject spawnPoint;
    public int minY = -10;
    public int breakParticleDensity = 10;
    public GameObject breakParticlePrefab;
    public int outlineParticleDensity = 10;
    public GameObject outlineParticlePrefab;
    // Yes, we could just use a dictionary but unity doesn't like dictionaries
    public LevelColor[] level;
    private List<SpriteRenderer> permanentObjectRenderers = new List<SpriteRenderer>();
    public LevelColor currentColor;
    public float bgColorLerpSpeed = 0.3f;
    public float fgColorLerpSpeed = 0.4f;
    public float growSpeed = 2f;
    public GameObject permanentParent;

    private Dictionary<GameObject, Vector3> growing = new Dictionary<GameObject, Vector3>();

    private GameObject player;
    private SpriteRenderer playerRenderer; // cache it so we don't need to getcomponent it every frame
    private SpriteRenderer spawnPointRenderer; // same here
    public Vector3 spawnPos;
    public TextMeshPro[] worldText;

    void Awake() {
        player = FindObjectOfType<PlayerController>().gameObject;
        playerRenderer = player.GetComponent<SpriteRenderer>();
        spawnPointRenderer = spawnPoint.GetComponent<SpriteRenderer>();
        spawnPos = spawnPoint.transform.position + new Vector3(0,1,0);
        currentColor = level[0];
        foreach (Transform child in permanentParent.transform) {
            permanentObjectRenderers.Add(child.gameObject.GetComponent<SpriteRenderer>());
        }
        LevelColor lastLevel = level[0];
        foreach (LevelColor lc in level) {
            lc.nextColor = lastLevel;
            lastLevel = lc;
            List<GameObject> children = new List<GameObject>();
            foreach (Transform child in lc.parentObject.transform){
                children.Add(child.gameObject);
                
            } 
            lc.objects = children.ToArray();
            foreach (GameObject o in lc.objects) {
                foreach (GameObject prefab in new GameObject[]{breakParticlePrefab, outlineParticlePrefab}) {
                    GameObject particleObject = Instantiate(prefab);
                    particleObject.name = prefab.name; // avoid the "(clone)" part
                    particleObject.transform.parent = o.transform;
                    particleObject.transform.localScale = new Vector3(1, 1, 1);
                    particleObject.transform.localPosition = new Vector3(0, 0, 0);
                }
                
                o.GetComponent<SpriteRenderer>().color = lc.scheme.main;
                Disable(o);
            }
        }
        level[0].nextColor = level[level.Length-1]; // theres probably a cleaner away also duplicated twice
        NextColor();
        player.transform.position = spawnPos;
    }

    void Update() {
        Camera.main.backgroundColor = Color.Lerp(Camera.main.backgroundColor, currentColor.scheme.background, bgColorLerpSpeed);
        Color fgColor = Color.Lerp(playerRenderer.color, currentColor.scheme.special, fgColorLerpSpeed);
        playerRenderer.color = fgColor;
        spawnPointRenderer.color = fgColor;
        foreach (TextMeshPro t in worldText) t.color = fgColor;
        Color permColor = Color.Lerp(permanentObjectRenderers[0].color, currentColor.scheme.permanent, fgColorLerpSpeed);
        foreach (SpriteRenderer r in permanentObjectRenderers) {
            r.color = permColor;
        }
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
        main.startColor = currentColor.nextColor.scheme.main; // big assumption here
        var burst = emission.GetBurst(0);
        burst.count = o.transform.lossyScale.x * o.transform.lossyScale.y * breakParticleDensity;
        emission.SetBurst(0, burst);
        particles.Play();
    }

    void StartOutlineParticles(GameObject o) {
        ParticleSystem particles = o.transform.Find(outlineParticlePrefab.name).gameObject.GetComponent<ParticleSystem>();
        var main = particles.main;
        var emission = particles.emission;
        // failed attempt to fix outlines on rotated blocks
        //var shape = particles.shape;
        //shape.rotation = o.transform.rotation.eulerAngles;
        //shape.scale = o.GetComponent<BoxCollider2D>().size;
        emission.rateOverTime = outlineParticleDensity * ((transform.lossyScale.x * 2) + (transform.lossyScale.y * 2));
        main.startColor = currentColor.nextColor.scheme.main; // big assumption here
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

        // Change end point color
        ParticleSystem particles = FindObjectOfType<EndPoint>().gameObject.GetComponent<ParticleSystem>();
        var main = particles.main;
        main.startColor = currentColor.scheme.special;

    }

    public void NextColor() {
        ChangeColor(currentColor.nextColor);
    }
}
