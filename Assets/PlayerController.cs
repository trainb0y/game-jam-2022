using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public float acceleration = 10;
	public float velocityCap = 3;
	public float airborneAccelMultiplier = 0.5f;
	public float jumpForce = 100;
	private Rigidbody2D rb;
	public bool grounded = true;

	void Awake(){rb = GetComponent<Rigidbody2D>();}

	void Update()
	{
		if (Input.GetKey(KeyCode.A)) Move(new Vector2(-acceleration * Time.deltaTime, 0));
		if (Input.GetKey(KeyCode.D)) Move(new Vector2(acceleration * Time.deltaTime, 0));
		if (Input.GetKey(KeyCode.W) && grounded)  {
			rb.AddForce(new Vector2(0, jumpForce * Time.deltaTime));
			grounded = false;
		}
	}

	void Move(Vector2 accel) {
		if (rb.velocity.magnitude > velocityCap && Mathf.Sign(accel.x) == Mathf.Sign(rb.velocity.x)) return;
		if (!grounded) accel *= airborneAccelMultiplier;
		rb.AddForce(accel);
	}
}
