using UnityEngine;

public enum JumpState {
    GROUNDED,
    JUMPING,
    AIRBORNE
}

public class PlayerController : MonoBehaviour
{
	public Vector2 minSize = new Vector2(0.5f, 0.5f);
	public Vector2 maxSize = new Vector2(1.5f, 1.5f);
	public float acceleration = 10;
	public float velocityCap = 3;
	public float airbornAccelMultiplier = 0.5f;
	public float jumpForce = 100;
	public float jumpBoostForce = 5;
	public int jumpBoostMax = 20;
	[SerializeField()]
	private JumpState jumpState = JumpState.GROUNDED;

	[SerializeField()]
	private int jumpCount = 0;
	private Rigidbody2D rb;

	void Awake(){rb = GetComponent<Rigidbody2D>();}

	void FixedUpdate()
	{
		// Handle resetting jump
		Vector2 checkPos = new Vector2(transform.position.x, transform.position.y - ((transform.localScale.y / 2)) - 0.1f);
		// checkPos is the position to check whether it is in the ground. hopefully this works for all sizes
		if (Physics2D.OverlapPointAll(checkPos).Length > 0) jumpState = JumpState.GROUNDED;
        else if (jumpState != JumpState.JUMPING) jumpState = JumpState.AIRBORNE ; // reset jump state

		if (Input.GetKey(KeyCode.A)) Move(new Vector2(-acceleration * Time.fixedDeltaTime, 0));
		if (Input.GetKey(KeyCode.D)) Move(new Vector2(acceleration * Time.fixedDeltaTime, 0));
		if (Input.GetKey(KeyCode.W) && jumpState != JumpState.AIRBORNE)  Jump(); 
		//ChangeSize();
	}

	void ChangeSize() {
		Vector2 velocity = rb.velocity.normalized * 2;
		Vector2 size = new Vector2(Mathf.Abs(velocity.x), Mathf.Abs(velocity.y));
		transform.localScale = Util.Vector2Clamp(size, minSize, maxSize);
	}

	void Move(Vector2 accel) {
		if (rb.velocity.magnitude > velocityCap && Mathf.Sign(accel.x) == Mathf.Sign(rb.velocity.x)) return;
		if (jumpState != JumpState.GROUNDED) accel *= airbornAccelMultiplier;
		rb.AddForce(accel);
	}

	void Jump(){
		if (jumpState == JumpState.JUMPING) {
			if (jumpCount > jumpBoostMax) {
				jumpState = JumpState.AIRBORNE;
			}
			rb.AddForce(new Vector2(0, jumpBoostForce * Time.fixedDeltaTime));
			jumpCount++;
		}
		if (jumpState == JumpState.GROUNDED) {
			rb.AddForce(new Vector2(0, jumpForce * Time.fixedDeltaTime));
			jumpState = JumpState.JUMPING;
			jumpCount = 0;
		}
	}
}
