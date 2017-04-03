using UnityEngine;
using System.Collections;

public class SentryController : WeaponController {

	private Transform target;
	public float rotateSpeed;
	public float range = 100f;
	public int health = 100;
	private Quaternion lookRotation;
	private Vector3 direction;
	private int playerSet;

	void Start() {
		if (rotateSpeed < 1f)
			rotateSpeed = 1f;
		if (GameObject.Find ("PlayerEnt"))
			setPlayer ();
		source = GetComponent<AudioSource> ();
	}

	void Update(){
		if (playerSet == 0)
			setPlayer ();
		direction = (target.position - transform.position).normalized;
		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		lookRotation = Quaternion.AngleAxis (angle, Vector3.forward);
		transform.rotation = Quaternion.Slerp(transform.rotation,lookRotation,Time.deltaTime * rotateSpeed);
		playerCheck ();
	}
		
	void playerCheck(){
		RaycastHit2D hit = Physics2D.Raycast(firePoint.position, firePoint.right, range, canBeShot);
		if (hit && LayerMask.LayerToName (hit.transform.gameObject.layer) == "Character") {
			Fire ();
		}
	}

	public void applyDamage(int damage)
	{
		health -= damage;
		if (health <= 0)
			Destroy (gameObject);
	}

	public void setPlayer()
	{
		if(target = GameObject.Find("PlayerEnt").transform)
		playerSet = 1;
	}
}
