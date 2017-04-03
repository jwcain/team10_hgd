using UnityEngine;
using System.Collections;

public class BearTrapScript : MonoBehaviour {

	public Sprite shutSprite = null;
	public float secondHoldTime = 2.0f;
	public float health = 20f;
	public float destroyHealth = -50f;

	private Vector3 stuckPos;
	private float stuckTimeLeft = 0;
	private bool triggered = false;
	private GameObject playerRef = null;
	private bool disabled = false;

	// Use this for initialization
	void Start () {
		if (shutSprite == null) {
			Debug.LogError("There is no shut sprite for the bear trap");
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (triggered && !disabled) { //short circut this better?
			if (stuckTimeLeft > 0) {
				stuckTimeLeft -= Time.deltaTime;
				playerRef.transform.position = stuckPos; //lock the player in place
			} else {
				//Prevent the player from building up velocity while trapped
				Rigidbody2D rb = playerRef.GetComponent<Rigidbody2D>();
				if (rb != null) {
					rb.velocity = Vector2.zero;
					rb.angularVelocity = 0f;
				}
				//Delete the bear trap
				Destroy(this.gameObject);
			}
		}
	}
	//react to being shot
	public void applyDamage(int damage) {
		health -= damage;
		if (health <= destroyHealth) {
			Destroy (this.gameObject);
		}
		if (health <= 0) {
			disabled = true;
			this.gameObject.GetComponent<SpriteRenderer>().sprite = shutSprite;
		}
	}

	void OnCollisionEnter2D(Collision2D coll) {
		//print ("You hit the bearTrap");
		if (!triggered) {
			stuckTimeLeft = secondHoldTime;
			playerRef = coll.gameObject;
			stuckPos = playerRef.transform.position;
			triggered = true;
			this.gameObject.GetComponent<SpriteRenderer>().sprite = shutSprite;
		}
	}
}
