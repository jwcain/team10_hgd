using UnityEngine;
using System.Collections;

public class FanScript : MonoBehaviour {

	public int numberOfDivisions = 8; //number of raycasts to do to find points
	public int effectDistance = 30; //max distance a raycast can travel
	public float force = 20f;
	public LayerMask stopsFan;
	private int ppu = 32;
	public bool recalculateHitbox = false;
	public int recalulateIntervalMS = 200;
	private float storedTimeMS = 0f;

	// Use this for initialization
	void Start () {
		calculateAndUpdateCollider ();
	}

	void calculateAndUpdateCollider() {
		int width = this.gameObject.GetComponent<SpriteRenderer> ().sprite.texture.width;
		int height = this.gameObject.GetComponent<SpriteRenderer> ().sprite.texture.height;
		int halfW = width / 2; //warning loss of percision
		int halfH = height / 2; //warning loss of percision
		float halfWA = (float)halfW / (float)ppu;
		float halfHA = (float)halfH / (float)ppu;
		float heightA = (float)height / (float)ppu;
		//Vector2[] points = new Vector2[this.numberOfPoints + 4];
		Vector2[] points = new Vector2[4 + this.numberOfDivisions + 1];
		points [0] = new Vector2 (halfWA, heightA);
		points [1] = new Vector2 (halfWA, 0);
		points [2] = new Vector2 (-halfWA, 0);
		points [3] = new Vector2 (-halfWA, heightA);
		float distanceOffset = ((float)width / (float)this.numberOfDivisions) / (float)ppu;
		//Quaternion rotation = this.gameObject.transform.localRotation; //maybe not local rotation?
		PolygonCollider2D coll = this.GetComponent<PolygonCollider2D> ();
		for (int i = 0; i <= this.numberOfDivisions; i++) {
			Vector3 rayPosInLocal = new Vector3 (-halfWA + i * distanceOffset, halfHA, 0);
			//RaycastHit2D hit = Physics2D.Raycast (new Vector2(-halfWA + this.gameObject.transform.position.x + i * distanceOffset, this.gameObject.transform.position.y),/* rotation * */ this.gameObject.transform.up, this.effectDistance, this.stopsFan);
			RaycastHit2D hit = Physics2D.Raycast (this.gameObject.transform.TransformPoint(rayPosInLocal), this.gameObject.transform.up, this.effectDistance, this.stopsFan);
			float hitDis = hit.distance;
			if (hitDis == 0 && hit.transform == null)
				hitDis = this.effectDistance;
			//Debug.Log ("hitscan " + i + ": setting distance = " + hitDis);
			points [4 + i] = new Vector2(-halfWA + i * distanceOffset, hitDis + halfHA);
		}
		coll.SetPath (0, points);

	}

	void OnTriggerStay2D(Collider2D other) {
		Rigidbody2D phy = other.gameObject.GetComponent<Rigidbody2D> ();
		if (phy == null) return; //not physics object so skip
		float dist = Vector2.Distance(this.gameObject.transform.position, other.gameObject.transform.position);
		float percentage = 1 - (dist / this.effectDistance);
		//Debug.Log(other.name + ": force % = " + percentage);
		phy.AddForce (this.gameObject.transform.up * percentage * this.force * Time.deltaTime * 1000);
	}

	// Update is called once per frame
	void Update () {
		if (this.recalculateHitbox) {
			this.storedTimeMS += Time.deltaTime * 1000;
			if (this.storedTimeMS > this.recalulateIntervalMS) {
				this.storedTimeMS = 0;
				calculateAndUpdateCollider ();
			}
		}
	}
}
