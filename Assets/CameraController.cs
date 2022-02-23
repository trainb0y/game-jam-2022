using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float smoothTime = 0.3f;
    public Transform objectToFollow;
    public Vector3 objectFollowOffset;

    private Vector3 velocity = Vector3.zero;

    void LateUpdate(){
        Vector3 offsetPos = objectToFollow.position + objectFollowOffset;
        Vector3 destPos = new Vector3(offsetPos.x, offsetPos.y, transform.position.z);
        transform.position = Vector3.SmoothDamp(transform.position,  destPos, ref velocity, smoothTime);
   
    }
}
