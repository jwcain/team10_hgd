using UnityEngine;
using System.Collections;

public class WeaponController : MonoBehaviour {

	public float variance;
	public int ammo;
	public int damageDone = 30;
	public float fireRate = 0.5F;
	public LayerMask canBeShot;

	public Transform firePoint;

	protected float nextFire = 0.0F;
	protected Vector3 shootDir;
	public int projSpeed;

	// Used to play sounds
	protected AudioSource source;
	public AudioClip gunshot;

	public Projectile shotObject;
	public GameObject bulletTrailPrefab;

	// Use this for initialization
	void Start () {
		firePoint = transform.Find ("firePoint").transform;
		source = GetComponent<AudioSource> ();
		if (firePoint == null) {
			Debug.Log ("No firePoint found");
		}
	}

	public virtual void Fire () {
		if (ammo > 0) {
			if (Time.time > nextFire) {
				// For calculating Variance in shooting to emulate bullet spread
				// Multiply by 0.01 so that the values can be changed easily
				shootDir = firePoint.right;
				shootDir.y += Random.Range (-1 * variance * 0.01F, variance * 0.01F);

				nextFire = Time.time + 1 / fireRate;
				ammo--;
				source.PlayOneShot (gunshot, 1.0f);
				if (!shotObject)
					shootRay ();
				else
					shootProjectile ();
			}
		}
	}

	protected virtual void shootRay () {
		// Ray2D rShot = new Ray2D (firePoint.position, firePoint.right * 100);
		Debug.DrawRay (firePoint.position, shootDir * 100, Color.red);

		RaycastHit2D hit = Physics2D.Raycast (firePoint.position, shootDir, 100, canBeShot);
		if (hit) {
			generateTrail (hit.distance);
			print ("Hit: " + hit.transform.name);

			hit.transform.gameObject.SendMessage ("applyDamage", this.damageDone, UnityEngine.SendMessageOptions.DontRequireReceiver);
		} else {
			generateTrail ();
		}
	}

	protected virtual void generateTrail(float distance = -1F)
	{
		float angle = Mathf.Atan2(shootDir.y, shootDir.x) * Mathf.Rad2Deg;
		Quaternion lookRotation = Quaternion.AngleAxis (angle, Vector3.forward);
		GameObject bulletObj = Instantiate (bulletTrailPrefab, firePoint.position, lookRotation) as GameObject;
		BulletTrail trail = bulletObj.GetComponent<BulletTrail> ();
		trail.distance = distance;
		trail.startPos = firePoint.position;
	}

	protected virtual void shootProjectile () {
		Projectile clone = (Projectile) Instantiate (shotObject, firePoint.position, firePoint.rotation);
		clone.GetComponent<Rigidbody2D> ().AddForce (shootDir * projSpeed);
	}
}
