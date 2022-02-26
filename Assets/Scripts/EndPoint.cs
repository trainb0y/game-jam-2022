using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndPoint : MonoBehaviour
{
    public float delay;
    private float counter = 1;
    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.GetComponent<PlayerController>() != null) {
            // move to next scene
            counter += Time.deltaTime;
        }
    }

    void Update() {
        if (counter > 0){
            counter += Time.deltaTime;
            if (counter > delay) {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }
    }
}
