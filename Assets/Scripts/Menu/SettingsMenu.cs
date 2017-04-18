using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour {

	public GameObject[] characters;
	public GameObject[] CharBG;
	public Color[] availColors;
	public int[] colorSelected;
	public int numControllers;

	public Sprite b_lb;
	public Sprite b_rb;
	public Sprite b_x;
	public Sprite b_y;

	int selected = 0;
	bool canInteract;
	bool twoControllers;
	int rounds;
	bool randomMap;

	Text controllerText;
	Text roundText;
	Text mapText;
	float limiter = 0;

	void Start()
	{
		string[] joysticks = Input.GetJoystickNames ();
		colorSelected = new int[joysticks.Length];
		canInteract = true;
		twoControllers = true;
		rounds = 3;
		randomMap = false;
		colorSelected = new int[] { 0, 0 };
		controllerText = GameObject.Find ("ControllerText").GetComponent<Text> ();
		roundText = GameObject.Find ("RoundText").GetComponent<Text> ();
		mapText = GameObject.Find ("MapText").GetComponent<Text> ();
	}

	void Update()
	{
		float leftX = Input.GetAxisRaw ("L_XAxis_1");
		float leftY = Input.GetAxisRaw ("L_YAxis_1");
		if ((Mathf.Abs(leftX) > 0.1f || Mathf.Abs(leftY) > 0.1f) && canInteract) {
			canInteract = false;
			StartCoroutine (SelectionChange (leftX, leftY));
		}

		if (Input.GetButtonDown ("RB_1")) {
			if (colorSelected[0] < availColors.Length - 1) {
				colorSelected[0]++;
			}
		}
		if (Input.GetButtonDown ("LB_1")) {
			if (colorSelected[0] > 0) {
				colorSelected[0]--;
			}
		}
		if (Input.GetButtonDown ("RB_2") || (!twoControllers && Input.GetButtonDown ("Y_1"))) {
			if (colorSelected[1] < availColors.Length - 1) {
				colorSelected[1]++;
			}
		}
		if (Input.GetButtonDown ("LB_2") || (!twoControllers && Input.GetButtonDown ("X_1"))) {
			if (colorSelected[1] > 0) {
				colorSelected[1]--;
			}
		}
		UpdateCharacters ();


		if (Input.GetButtonDown ("Start_1")) {
			MatchSettingsHolder msh = GameObject.Find ("SettingsHolder").GetComponent<MatchSettingsHolder> ();
			msh.player1Color = new Color(availColors [colorSelected [0]].r, availColors [colorSelected [0]].g, availColors [colorSelected [0]].b, 1f);
			msh.player2Color = new Color(availColors [colorSelected [1]].r, availColors [colorSelected [1]].g, availColors [colorSelected [1]].b, 1f);
			msh.useTwoControllers = twoControllers;
			msh.rounds = rounds;
			msh.randomMaps = randomMap;
			if (randomMap) {
				SceneManager.LoadScene ("FinalGame");
			} else {
				SceneManager.LoadScene ("MapSelection");
			}
		}

	}

	void UpdateCharacters()
	{
		for (int i = 0; i < numControllers; i++) 
		{
			characters [i].GetComponent<SpriteRenderer> ().color = new Color(availColors [colorSelected [i]].r, availColors [colorSelected [i]].g, availColors [colorSelected [i]].b, 1f);
			CharBG [i].GetComponent<SpriteRenderer> ().color = new Color(availColors [colorSelected [i]].r, availColors [colorSelected [i]].g, availColors [colorSelected [i]].b, 0.25f);
		}
	}

	IEnumerator SelectionChange(float xAxis, float yAxis)
	{
		if (yAxis > 0.1f) {
			selected++;
			if (selected >= 3)
				selected = 0;
		}
		if (yAxis < -0.1f) {
			selected--;
			if (selected <= -1)
				selected = 2;
		}

		switch (selected) {
		case 0:
			controllerText.color = Color.green;
			roundText.color = Color.white;
			mapText.color = Color.white;

			if (Mathf.Abs(xAxis) > 0.1f)
				twoControllers = !twoControllers;
			
			if (twoControllers) {
				controllerText.text = "< Two Controllers >";
				GameObject.Find ("MoveLeft 2").GetComponent<SpriteRenderer> ().sprite = b_lb;
				GameObject.Find ("MoveRight 2").GetComponent<SpriteRenderer> ().sprite = b_rb;
			} else {
				controllerText.text = "< Pass One Controller >";
				GameObject.Find ("MoveLeft 2").GetComponent<SpriteRenderer> ().sprite = b_x;
				GameObject.Find ("MoveRight 2").GetComponent<SpriteRenderer> ().sprite = b_y;
			}
			break;
		case 1:
			controllerText.color = Color.white;
			roundText.color = Color.green;
			mapText.color = Color.white;

			if (xAxis > 0.1f) {
				rounds++;
				if (rounds > 10)
					rounds = 1;
			}
			if (xAxis < -0.1f) {
				rounds--;
				if (rounds < 1)
					rounds = 10;
			}
			roundText.text = "< " + rounds + " >";
			break;
		case 2:
			controllerText.color = Color.white;
			roundText.color = Color.white;
			mapText.color = Color.green;

			if (Mathf.Abs(xAxis) > 0.1f)
				randomMap = !randomMap;

			if (randomMap)
				mapText.text = "< Random >";
			else
				mapText.text = "< Select >";
			break;
		}

		yield return new WaitForSeconds (0.2f);
		canInteract = true;
	}
}
