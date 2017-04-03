using UnityEngine;
using System.Collections;

public class LandmineScript : MonoBehaviour {
	private Transform explodePoint;
	public float damage = 30f;
	public float health = 20f;
	// Use this for initialization
	void Start () {
		explodePoint = transform.Find ("explodePoint").transform;

	}
	
	// Update is called once per frame
	void Update () {
	
	}
	//react to being shot
	public void applyDamage(int damage) {
		health -= damage;
		if (health <= 0) {
			this.SendMessage ("doExplode");
			Destroy (this.gameObject);
		}
	}

	void OnCollisionEnter2D(Collision2D coll) {
		//tell the attached ExplodableObj script to run the explosion forces
		this.SendMessage("doExplode");
		//tell the thing that collided with the landmine to apply damage to itself
		coll.gameObject.SendMessage ("applyDamage", this.damage, UnityEngine.SendMessageOptions.DontRequireReceiver);
		//now remove yourself
		Destroy(this.gameObject);
	}
}
