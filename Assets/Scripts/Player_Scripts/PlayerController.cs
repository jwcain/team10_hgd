using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController: MonoBehaviour {

	// Player Attributes
	public float resistance = 1f;
	public float xAccel = 0.75f;
	public float maxSpeed = 30f;
	public float jumpForce = 2000f;
	public float startingHealth = 100F;
	public float currentHealth;
	public bool onLadder = false;
	public string powerUp;

	// Player Default Attributes
	// Is based on the player's values at Start
	private float default_resistance;
	private float default_maxSpeed;
	private float default_jumpForce;
	private float default_gravityScale;

	private int contToUse;

	bool facingRight = true;

	// References
	Animator[] anims; // This is currently not used because I do not have animations yet
	Rigidbody2D rBody;
	public WeaponController currentWeapon;
	public WeaponController defaultWeapon;
	public PlayerHud ui;

	// For Ground collision
	bool onGround = false;
	public Transform groundCheck;
	float groundRadius = 0.2f;
	public LayerMask whatIsGround;

	private bool canTakeDamage = true;
	public float blink_speed = 0.3f;
	public float invincible_duration = 3f;

	// Used to play sounds
	protected AudioSource source;
	public AudioClip playerHitSound;
	public AudioClip playerPickUpSound;
	public AudioClip pickUpPowerSound;

	void Awake()
	{
		source = GetComponent<AudioSource> ();
	}

	// Use this for initialization
	void Start () {
		default_resistance = resistance;
		default_maxSpeed = maxSpeed;
		default_jumpForce = jumpForce;
		default_gravityScale = this.GetComponent<Rigidbody2D> ().gravityScale;
		onLadder = false;

		contToUse = 1;

		anims = GetComponentsInChildren<Animator> ();
		rBody = GetComponent<Rigidbody2D> ();
		if (GameObject.Find("PlayerUI"))
			ui = GameObject.Find ("PlayerUI").GetComponent<PlayerHud>();

		currentHealth = startingHealth;
		if (ui) ui.updateHealth ();

		if (defaultWeapon == null) {
			Debug.LogError ("No Default Weapon Set in PlayerController");
		}

		if (currentWeapon == null)
			currentWeapon = Instantiate (defaultWeapon);
		setWeapon ();
	}
	
	// Update is called once per frame
	void Update () {
		if ((onGround || onLadder) && (Input.GetButtonDown("A_" + contToUse) || Input.GetKeyDown("space"))) {
			rBody.AddForce (new Vector2 (0f, jumpForce));
			onGround = false;
		}
		// Shooting
		if (currentWeapon != null && Input.GetAxis ("TriggersR_" + contToUse) < 0) {
			currentWeapon.Fire ();
			if (currentWeapon.ammo == 0)
				pickUpWeapon (Instantiate(defaultWeapon));
		}
		/*
		if (Input.GetKey("a"))
			transform.Translate(-Vector3.right * maxSpeed * Time.deltaTime);
		if (Input.GetKey("d"))
			transform.Translate(-Vector3.left * maxSpeed * Time.deltaTime);*/
	}

	void FixedUpdate () {
		onGround = Physics2D.OverlapCircle (groundCheck.position, groundRadius, whatIsGround);

		float inputDirection = Input.GetAxis ("L_XAxis_" + contToUse);
		// Calculate how much the velocity should change based on xAccel
		float velChange = inputDirection * xAccel;
		float newXVelocity, newYVelocity;

		if (velChange != 0f) {
			// Add to the current velocity
			newXVelocity = rBody.velocity.x + velChange;
			if (onGround) {
				PlayAnimation ("Walking");
			}
		} else { 
			// Stop completely if there's no input
			newXVelocity = 0f;
			if (onGround)
				PlayAnimation ("Idle");
		}
		// Limit the max velocity
		newXVelocity = Mathf.Clamp(newXVelocity, -maxSpeed, maxSpeed);

		if (onLadder) {
			float inputY = Input.GetAxis ("L_YAxis_" + contToUse);
			float yVelChange = -1 * inputY * xAccel;
			if (yVelChange != 0f) {
				// Add to the current velocity
				newYVelocity = rBody.velocity.y + yVelChange;
			} else { 
				// Stop completely if there's no input
				newYVelocity = 0f;
				rBody.gravityScale = 0f;
			}
			newYVelocity = Mathf.Clamp(newYVelocity, -maxSpeed, maxSpeed);
		} else {
			newYVelocity = rBody.velocity.y;
		}

		// Apply the new velocity
		rBody.velocity = new Vector2 (newXVelocity, newYVelocity);


		// Update the speed of the walking animation
		for (int i=0; i<anims.Length; i++)
			anims[i].SetFloat ("Speed", Mathf.Abs(newXVelocity*2)/maxSpeed);

		// Make sure the character is facing the right direction
		if (inputDirection > 0 && !facingRight)
			Flip ();
		else if (inputDirection < 0 && facingRight)
			Flip ();

		// Should start falling animation?
		if (!onGround)
			PlayAnimation ("Falling");
	}

	void PlayAnimation(string name) {
		if(!anims[0].GetCurrentAnimatorStateInfo(0).IsName(name))
			for (int i=0; i<anims.Length; i++){
				anims[i].Play (name, 0, i*0.5f); //Use i*0.5 to offset the second foot
			}
	}
		
	void Flip () {
		facingRight = !facingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	// Getters and Setters
	public int getDirectionFacing () {
		if (facingRight)
			return 1;
		else
			return -1;
	}

	public void pickUpWeapon (WeaponController newWeapon) {
		currentWeapon = newWeapon;
		setWeapon ();
		source.PlayOneShot (playerPickUpSound, 1f);
	}

	public void setWeapon () {
		Transform hands = transform.Find ("hands");
		for (int i = 0; i < hands.childCount; i++) {
			Transform weapon = hands.GetChild (i);
			weapon.parent = null;
			Destroy (weapon.gameObject);
		}
		currentWeapon.transform.position = hands.position;
		currentWeapon.transform.rotation = hands.rotation;
		currentWeapon.transform.parent = hands;
		// Make sure that the localScale after you attach the weapon 1, 1, 1
		//  Relative to the parent which is the hands
		currentWeapon.transform.localScale = new Vector3(1,1,1);
	}

	// Changes the player's health by @param damage
	/*	If the damage <= 0 we do not run coroutine takenDamage
	 *	cause no damage was taken.
	 */
	public void applyDamage(float damage) {
		if (canTakeDamage) {
			// Calculate the damage taken
			float actualDamage;
			actualDamage = damage * resistance;

			currentHealth = Mathf.Clamp (currentHealth - actualDamage, 0, startingHealth);

			source.PlayOneShot (playerHitSound, 1.0f);

			if (damage > 0) 
				StartCoroutine ("takenDamage");

			if (ui) ui.updateHealth ();
		}
	}

	// Alias Method for applyDamage
	public void modifyHealth(float damage) {
		applyDamage (damage);
	}

	// Coroutine for showing damage
	IEnumerator takenDamage()
	{
		SpriteRenderer[] char_sprites = GetComponentsInChildren<SpriteRenderer> ();
		Color color_with_opacity = new Color (1f, 1f, 1f, 0.7f);
		Color color_without_opacity = new Color (1f, 1f, 1f, 1f);
		int number_of_blinks = (int) (invincible_duration / blink_speed);
		canTakeDamage = false;

		foreach (SpriteRenderer rend in char_sprites) {
			if (rend)
				rend.color = Color.red;
		}
		yield return new WaitForSeconds(0.3f);
		foreach (SpriteRenderer rend in char_sprites) {
			if (rend)
				rend.color = Color.white;
		}
		int count = 0;
		while (count < number_of_blinks) {
			foreach (SpriteRenderer rend in char_sprites) {
				if (rend)
					rend.color = color_with_opacity;
			}
			yield return new WaitForSeconds(blink_speed);
			foreach (SpriteRenderer rend in char_sprites) {
				if (rend)
					rend.color = color_without_opacity;
			}
			yield return new WaitForSeconds(blink_speed);
			count++;
		}
		canTakeDamage = true;
	}

	// Used for powerup management
	public void powerUpUntil(int duration, Sprite puImage)
	{
		source.PlayOneShot (pickUpPowerSound, 1f);
		ui.applyPowerUp (duration, puImage);
		StartCoroutine (powerUpUntilRoutine (duration));
	}

	// Resets everything on the player
	// Mainly done so because of invicibility remaining on the player
	//	if you disable to the player in the middle of its execution
	public void resetEverything()
	{
		resetAttributesToDefault ();
		resetHealthOfPlayer ();
		SpriteRenderer[] char_sprites = GetComponentsInChildren<SpriteRenderer> ();
		foreach (SpriteRenderer rend in char_sprites) {
			rend.color = Color.white;
		}
		canTakeDamage = true;
	}

	// Sets the player attributes to its default values
	public void resetAttributesToDefault()
	{
		if(!powerUp.Equals("resistance"))
			resistance = default_resistance;
		if(!powerUp.Equals("movement"))
			maxSpeed = default_maxSpeed;
		if(!powerUp.Equals("jump"))
			jumpForce = default_jumpForce;
		if(!powerUp.Equals("gravity"))
			GetComponent<Rigidbody2D>().gravityScale = default_gravityScale;
		onLadder = false;
		rBody.gravityScale = 9.8f;
	}

	public void resetHealthOfPlayer ()
	{
		currentHealth = startingHealth;
		ui.updateHealth ();
	}

	public IEnumerator powerUpUntilRoutine(float duration)
	{
		yield return new WaitForSeconds (duration);
		powerUp = "";
		resetAttributesToDefault ();
	}

	public void setController(int contID)
	{
		contToUse = contID;
		RotateTowardsInput[] rotatingParts = transform.GetComponentsInChildren<RotateTowardsInput> ();
		foreach (RotateTowardsInput part in rotatingParts)
			part.setController (contToUse);
	}
}
