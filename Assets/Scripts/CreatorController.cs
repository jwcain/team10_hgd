using UnityEngine;
using System.Collections.Generic;

public class CreatorController : MonoBehaviour {

	public List<Trap> availableObjs;
	public float moveSpeed;
	public int money;
	// Used to get reference to see who is the current player
	private GameController game;
	private GameDebugController gameDebug;
	private int currObj;
	private Transform currObjRenderer;
	private Transform snappedEdge;
	private Vector3 snappedEdgePos;
	private bool canPlace;

	private int contToUse;

	public CreatorHud ui;

	protected AudioSource source;
	public AudioClip spawnObjectSound;

	void Awake()
	{
		source = GetComponent<AudioSource> ();
	}

	// Use this for initialization
	void Start () {

		if (GameObject.Find ("Game")) {
			game = GameObject.Find ("Game").GetComponent<GameController> ();
		} else {
			gameDebug = GameObject.Find ("GameDebug").GetComponent<GameDebugController> ();
		}

		//Load the available traps from the traps resouce folder
		availableObjs = new List<Trap> ();
		Object[] objs = Resources.LoadAll("Traps/");
		foreach(Object obj in objs) {
			if(((GameObject)obj).GetComponent<Trap>() != null)
				availableObjs.Add(((GameObject)obj).GetComponent<Trap>());
		}

		// print (game.player);
		currObj = 0;
		contToUse = 1;
		currObjRenderer = transform.Find ("currentObj");
		setObjRenderer ();
	}

	// Update is called once per frame
	void Update () {
		if (!ui) {
			ui = GameObject.Find ("CreatorUI").GetComponent<CreatorHud>();
			ui.updateMoneyText(money);
			setObjRenderer ();
		}
		float inputXAmount = Input.GetAxis ("L_XAxis_" + contToUse);
		float inputYAmount = Input.GetAxis ("L_YAxis_" + contToUse);

		if (Input.GetButtonDown ("A_" + contToUse))
			spawnGameObject ();

		if (Input.GetButtonDown ("RB_" + contToUse)) {
			if (currObj < availableObjs.Count - 1)
				currObj++;
			else
				currObj = 0;
			setObjRenderer ();
		}
		if (Input.GetButtonDown ("LB_" + contToUse)) {
			if (currObj > 0)
				currObj--;
			else
				currObj = availableObjs.Count - 1;
			setObjRenderer ();
		}
		
		// Calculate how much the velocity should change based on xAccel
		Vector3 direction = new Vector3 (inputXAmount, -inputYAmount, 0.0f);
		transform.Translate (moveSpeed * direction * Time.deltaTime);

		GetComponent<CircleCollider2D> ().attachedRigidbody.WakeUp();

		Trap thisObj = availableObjs [currObj];
		// Reset the snapped object if out of range of any cubes
		LayerMask platforms = LayerMask.GetMask("Platforms");

		if (snappedEdge == null ||!GetComponent<CircleCollider2D> ().IsTouchingLayers (platforms) || 
			Input.GetButton ("Y_" + contToUse) || !thisObj.canPlaceOnWalls) {
			// Reset the object snapped to
			snappedEdge = null;
			currObjRenderer.localPosition = new Vector3 (0, 0, 0);
			currObjRenderer.eulerAngles = new Vector3 (0, 0, 0);
		} else {
			/* Update currObjectRenderer's position/rotation on the snapped edge
			 * manually, as this may not be called every frame. This prevents the
			 * object from being shaky when moving. */
			if(snappedEdge != null)
				OnTriggerStay2D (snappedEdge.gameObject.GetComponent<BoxCollider2D> ());
		}
			
		//Check if you can place this object right now
		canPlace = (thisObj.canPlaceInAir || snappedEdge != null);
		//Check if this object is being placed on another
		if (currObjRenderer.GetComponent<Collider2D> ().IsTouchingLayers (LayerMask.GetMask ("Creator", "Enemies"))) {
			canPlace = false;
		}
		//Check if a laser hits something
		if (thisObj.name.Equals ("Laser") && !LaserHead.checkLaser (currObjRenderer.transform.position, 
			currObjRenderer.transform.rotation))
			canPlace = false;
		
		Color color;
		if(canPlace)
			color = new Color(1f, 1f, 1f, 0.7f);
		else
			color = new Color(1f, 0f, 0f, 0.3f);
		currObjRenderer.GetComponent<SpriteRenderer> ().color = color;
	}

	void OnTriggerStay2D(Collider2D other) {
		// Don't snap to the edge if Y is held (debug)
		// TODO Remove this when needed
		if (Input.GetButton ("Y_" + contToUse))
			return;

		// Don't snap to an edge if an object cannot do so
		if (!availableObjs [currObj].canPlaceOnWalls)
			return;
		
		// Get the object that was collided with
		GameObject obj = other.gameObject;

		// Forget this object if it is not a platform object
		if (obj.layer != LayerMask.NameToLayer("Platforms"))
			return;

		// Find the closest point within the platform's bounds
		Vector3 objFacing = obj.transform.rotation * Vector3.up;

		Vector3 closestPos = Vector3.zero;
		Vector3 dir = objFacing;
		for (int i = 0; i < 4; i++) {
			RaycastHit2D hit = Physics2D.Raycast (transform.position, dir, 2, LayerMask.GetMask ("Platforms"));
			if (hit.collider != null) {
				closestPos = hit.point;
				break;
			}
			dir = Quaternion.Euler (0f, 0f, 90f) * dir;
		}

		if (closestPos == Vector3.zero) {
			snappedEdge = null;
			return;
		}

		// Calculate that point relative to the Creator
		Vector3 relativePos = closestPos - transform.position;
		float distance = relativePos.magnitude;
		// Forget this object if there is a closer platform than this one
		if (snappedEdge != null && snappedEdge != obj.transform && distance > snappedEdgePos.magnitude)
			return;

		// Update snappedEdge
		snappedEdge = obj.transform;
		snappedEdgePos = relativePos;

		// Update rotation
		Vector3 dirFace = Quaternion.Euler (0f, 0f, 90f) * dir;
		float rotationAngle = Mathf.Atan2 (dirFace.y, dirFace.x) * Mathf.Rad2Deg;
		currObjRenderer.eulerAngles = new Vector3(0, 0, rotationAngle);

		// Set currObjectRenderer's position to the edge's
		currObjRenderer.localPosition = snappedEdgePos;
	}

	private void spawnGameObject()
	{
		if (canPlace && availableObjs [currObj].cost <= money) {
			GameObject spawned = Instantiate (availableObjs [currObj].gameObject);
			spawned.transform.position = currObjRenderer.position;
			spawned.transform.rotation = currObjRenderer.rotation;
			money -= availableObjs [currObj].cost;
			ui.updateMoneyText (money);
			// Debug.Log ("Creator has created: " + spawned.name);
			if (spawned.GetComponent<SentryController> ())
				spawned.GetComponent<SentryController> ().enabled = false;

			if (game) {
				game.applyGameObject (spawned);
			} else {
				gameDebug.applyGameObject (spawned);
			}

			source.PlayOneShot (spawnObjectSound, 1f);
		}
	}

	private void setObjRenderer()
	{
		SpriteRenderer currSelectedSprite = availableObjs [currObj].gameObject.GetComponent<SpriteRenderer> ();
		currObjRenderer.GetComponent<SpriteRenderer>().sprite = currSelectedSprite.sprite;
		Color temp = currObjRenderer.GetComponent<SpriteRenderer> ().color;
		temp.a = 0.7f;
		currObjRenderer.GetComponent<SpriteRenderer> ().color = temp;
		currObjRenderer.localScale = availableObjs [currObj].transform.localScale;
		//Set the collider to math the sprite's bounds
		BoxCollider2D col = currObjRenderer.GetComponent<BoxCollider2D>();
		col.size = currSelectedSprite.sprite.bounds.size;
		col.offset = currSelectedSprite.sprite.bounds.center;
		// print (currObj);
		ui.updateObjectPreview (currObj);
	}

	public void setController(int contID)
	{
		contToUse = contID;
	}
}
