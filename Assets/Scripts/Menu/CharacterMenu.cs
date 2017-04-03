using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class CharacterMenu : MonoBehaviour {
	public int current;
	public GameObject[] characters;
	public GameObject[] CharBG;
	public Color[] availColors;
	//public Sprite[] characterSprite;
	//public Sprite[] characterBackground;
	public int[] selected;
	public int numControllers;

	GC2 gameController;
	GameObject PressA;
	bool[] canInteract;
	bool loadNextScene;
	float limiter = 0;

	void Start()
	{
		string[] joysticks = Input.GetJoystickNames ();
		selected = new int[joysticks.Length];
		canInteract = new bool[] { true, true };
		selected = new int[] { 0, 0 };
		//gameController = GameObject.Find ("Game");
		PressA = GameObject.Find ("PressAtoPlay");
		PressA.GetComponent<SpriteRenderer> ().color = Color.clear;
		loadNextScene = false;
	}

	void Update()
	{
		float input;
		for (int i = 0; i < numControllers; i++) {
			input = Input.GetAxisRaw ("L_XAxis_" + (i + 1));
			if (input != 0 && canInteract[i]) {
				canInteract[i] = false;
				StartCoroutine (SelectionChange (input, i));
			}
			if (Input.GetButtonDown("A_" + (i+1)))
			{
				if (loadNextScene == true) {	
					/*
					Debug.Log ("Ran.");
					gameController = GameObject.Find ("Game").GetComponent<GC2>();
					gameController.GetComponent<GC2>().player1Color = new Color(availColors [selected [0]].r, availColors [selected [0]].g, availColors [selected [0]].b, 1f);
					Debug.Log ("Ran.");
					gameController.GetComponent<GC2>().player2Color = new Color(availColors [selected [1]].r, availColors [selected [1]].g, availColors [selected [1]].b, 1f);
					Debug.Log ("Ran.");
					*/
					PlayerColorHolder plh = GameObject.Find ("ColorHolder").GetComponent<PlayerColorHolder> ();
					plh.player1Color = new Color(availColors [selected [0]].r, availColors [selected [0]].g, availColors [selected [0]].b, 1f);
					plh.player2Color = new Color(availColors [selected [1]].r, availColors [selected [1]].g, availColors [selected [1]].b, 1f);
					SceneManager.LoadScene ("FinalGame");

				} else {
					PressA.GetComponent<SpriteRenderer> ().color = Color.white;
					loadNextScene = true;
				}
			}
		}
	}

	void UpdateCharacters()
	{
		for (int i = 0; i < numControllers; i++) 
		{
			characters [i].GetComponent<SpriteRenderer> ().color = new Color(availColors [selected [i]].r, availColors [selected [i]].g, availColors [selected [i]].b, 1f);
			CharBG [i].GetComponent<SpriteRenderer> ().color = new Color(availColors [selected [i]].r, availColors [selected [i]].g, availColors [selected [i]].b, 0.25f);
		}
	}

	IEnumerator SelectionChange(float input, int controller)
	{
		loadNextScene = false;

		if (input > 0 && selected[controller] < availColors.Length - 1) {
			selected[controller]++;
		} else if (input < 0 && selected[controller] > 0) {
			selected[controller]--;
		}

		UpdateCharacters ();
		yield return new WaitForSeconds (0.2f);
		canInteract[controller] = true;
	}
}
