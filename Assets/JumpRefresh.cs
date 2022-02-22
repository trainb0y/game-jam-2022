using UnityEngine;
public class JumpRefresh : MonoBehaviour
{
    public PlayerController player;
    void FixedUpdate() {
        if (Physics2D.OverlapPointAll(transform.position).Length > 0) player.grounded = true;
        else player.grounded = false;
    }
}
