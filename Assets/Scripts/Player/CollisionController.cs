using UnityEngine;
using System.Collections;

public class CollisionController : MonoBehaviour {

	public bool isPlayer;
	public PlayerController player;
	public GameController game;

	void Start() {
		isPlayer = gameObject.tag.Equals ("Player");
		if(isPlayer)
			player = GetComponent<PlayerController> ();
		if(GameObject.Find("Game"))
			game = GameObject.Find ("Game").GetComponent<GameController> ();
	}

	//For objects that aren't triggers
	void OnCollisionStay2D(Collision2D other) {
		switch (other.gameObject.tag) {

		case "LaserBeam":
			gameObject.SendMessage ("applyDamage", 10f, UnityEngine.SendMessageOptions.DontRequireReceiver);
			break;
		case "Lava":
			gameObject.SendMessage ("applyDamage", 20f, UnityEngine.SendMessageOptions.DontRequireReceiver);
			break;
		case "Slime":
			if(isPlayer && !player.powerUp.Equals("jump"))
				player.jumpForce = 600f;
			break;
		}
	}

	//For objects set as triggers
	void OnTriggerStay2D(Collider2D other) {
		switch (other.gameObject.tag) {

		case "Obstacle_Damage":
			gameObject.SendMessage ("applyDamage", 10f, UnityEngine.SendMessageOptions.DontRequireReceiver);
			break;
		case "Saw":
			gameObject.SendMessage ("applyDamage", 30f, UnityEngine.SendMessageOptions.DontRequireReceiver);
			break;
		case "Ladder":
			if(isPlayer)
				player.onLadder = true;
			break;
		case "EndFlag":
			if(isPlayer)
				game.endPlayerPhase ();
			break;
		case "Spike":
			gameObject.SendMessage ("applyDamage", 10f, UnityEngine.SendMessageOptions.DontRequireReceiver);
			break;
		case "OutOfBounds":
			gameObject.SendMessage ("applyDamage", 1000f, UnityEngine.SendMessageOptions.DontRequireReceiver);
			break;
		case "SpiderCrawler":
			if(isPlayer)
				gameObject.SendMessage ("applyDamage", 10f, UnityEngine.SendMessageOptions.DontRequireReceiver);
			break;
		}
	}

	void OnCollisionExit2D(Collision2D col)
	{
		if(isPlayer)
			player.resetAttributesToDefault ();
	}

	void OnTriggerExit2D(Collider2D col)
	{
		if(isPlayer)
			player.resetAttributesToDefault ();
	}
}
