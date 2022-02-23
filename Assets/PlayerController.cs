using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public float acceleration = 10;
	public float velocityCap = 3;
	public float airbornAccelMultiplier = 0.5f;
	public float jumpForce = 100;
	public float jumpBoostForce = 5;
	public int jumpBoostMax = 20;
	public JumpState jumpState = JumpState.GROUNDED;
	private Rigidbody2D rb;
	private int jumpCount = 0;

	void Awake(){rb = GetComponent<Rigidbody2D>();}

	void FixedUpdate()
	{
		if (Input.GetKey(KeyCode.A)) Move(new Vector2(-acceleration * Time.fixedDeltaTime, 0));
		if (Input.GetKey(KeyCode.D)) Move(new Vector2(acceleration * Time.fixedDeltaTime, 0));
		if (Input.GetKey(KeyCode.W) && jumpState != JumpState.AIRBORNE)  Jump(); 
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
