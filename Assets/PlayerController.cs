using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public float baseFriction = 0.5f;
	public float frictionReduction = 0.3f;
	public float speed = 10;
	public float jumpForce = 100;
	private Rigidbody2D rb;
	public bool grounded = true;

	void Awake(){rb = GetComponent<Rigidbody2D>();}

	void Update()
	{
		if (Input.GetKey(KeyCode.A)) rb.AddForce(new Vector2(-speed * Time.deltaTime, 0));
		if (Input.GetKey(KeyCode.D)) rb.AddForce(new Vector2(speed * Time.deltaTime, 0));
		if (Input.GetKey(KeyCode.W) && grounded)  {
			rb.AddForce(new Vector2(0, jumpForce * Time.deltaTime));
			grounded = false;
		}
		
	}
}
