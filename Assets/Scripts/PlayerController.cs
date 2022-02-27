using UnityEngine;
using System.Collections.Generic;
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
	public float switchCooldown = 0.1f;
	[SerializeField()]
	private JumpState jumpState = JumpState.GROUNDED;

	[SerializeField()]
	private int jumpCount = 0;
	private Rigidbody2D rb;
	private LevelHandler lh;

	private float lastSwitch;
	private AudioSource jumpAudio;

	void Awake() {
		rb = GetComponent<Rigidbody2D>();
		lh = FindObjectOfType<LevelHandler>();
		jumpAudio = GetComponent<AudioSource>();
	}

	void FixedUpdate()
	{
		// Handle resetting jump
		Vector2 startPos = new Vector2(transform.position.x - (transform.lossyScale.x / 2), transform.position.y - ((transform.lossyScale.y / 2)) - 0.1f);
		Vector2 endPos = new Vector2(transform.position.x + (transform.lossyScale.x / 2), transform.position.y - ((transform.lossyScale.y / 2)) - 0.1f);
		// Debug.DrawLine(startPos, endPos);
		Physics2D.queriesHitTriggers=false; // we don't want to be able to jump off of triggers
		if (Physics2D.LinecastNonAlloc(startPos, endPos, new RaycastHit2D[1]) > 0) jumpState = JumpState.GROUNDED;
        else if (jumpState != JumpState.JUMPING) jumpState = JumpState.AIRBORNE ; // reset jump state
		Physics2D.queriesHitTriggers=true;

		if (Input.GetKey(KeyCode.Space)) {if (Time.time - lastSwitch > switchCooldown) { lh.NextColor(); lastSwitch = Time.time;}}
		if (Input.GetKey(KeyCode.A)) Move(new Vector2(-acceleration * Time.fixedDeltaTime, 0));
		if (Input.GetKey(KeyCode.D)) Move(new Vector2(acceleration * Time.fixedDeltaTime, 0));
		if (Input.GetKey(KeyCode.W) && jumpState != JumpState.AIRBORNE)  Jump(); 
		//ChangeSize();

		if (transform.position.y < lh.minY) {
			transform.position = lh.spawnPos;
			rb.velocity = Vector3.zero;
		}
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
			jumpAudio.Play();
		}
	}
}
