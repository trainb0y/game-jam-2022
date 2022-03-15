 using UnityEngine;
 using System.Collections;
 
 // Source: Unity Forum
 public class MusicPlayer : MonoBehaviour
 {
 
     private static MusicPlayer instance = null;
     public static MusicPlayer Instance
     {
         get { return instance; }
     }
     void Awake()
     {
         if (instance != null && instance != this) {
             Destroy(this.gameObject);
             return;
         } else {
             instance = this;
         }
         DontDestroyOnLoad(this.gameObject);
     }
 }