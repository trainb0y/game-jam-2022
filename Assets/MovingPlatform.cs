using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Vector3 startPos;
    public Vector3 endPos;
    public float speed;

    private Vector3 destination;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = startPos;
        destination = endPos;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, destination, speed*Time.deltaTime);
        if (Util.Vector3DistanceSquared(transform.position, destination) < 0.2f) {
            if (endPos == destination) destination = startPos;
            else destination = endPos;
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.GetComponent<PlayerController>() != null) {
            other.transform.parent = transform;
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.GetComponent<PlayerController>() != null) {
            other.transform.parent = null;
        }
    }
}
