using UnityEngine;
public class JumpRefresh : MonoBehaviour
{
    public PlayerController player;
    void FixedUpdate() {
        if (Physics2D.OverlapPointAll(transform.position).Length > 0) player.jumpState = JumpState.GROUNDED;
        else if (player.jumpState != JumpState.JUMPING) player.jumpState = JumpState.AIRBORNE ; // reset jump state
    }
}
