using UnityEngine;
using System.Collections;

public class ExplodableObj : MonoBehaviour {
	//Transform explosionPoint;
	public float maxR = 1000;
	public float force = 2;
	public float dropOffRate = 1;
	public LayerMask mask;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void doExplode() {
		Collider2D[] hitColliders = Physics2D.OverlapCircleAll(this.gameObject.transform.position, this.maxR, this.mask);
		//apply force to each object
		foreach (Collider2D c in hitColliders) {
			Rigidbody2D phy = c.GetComponent<Rigidbody2D> ();
			if (phy == null) continue; //not physics object so skip
			//add the force
			Vector3 calc = (c.gameObject.transform.position - this.gameObject.transform.position);
			calc.z = 0f;
			//print( "applying explosion force to " + c.gameObject.name + ". The force appied is: " + calc);
			phy.AddForce (calc * this.force, ForceMode2D.Impulse);
		}
	}
}
