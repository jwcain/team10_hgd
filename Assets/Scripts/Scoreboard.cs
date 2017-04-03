using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Scoreboard : MonoBehaviour {

	public Sprite[] roles;
	public SpriteRenderer[] roleSprites;

	private Text timerText;
	private Text roundText;
	private Text p1ScoreText;
	private Text p2ScoreText;
	private Text infoText;
	public Image p1Role;
	public Image p2Role;
	private GameObject loserTextsP1;
	private GameObject loserTextsP2;
	private Color color1;
	private Color color2;



	// Use this for initialization
	void Awake () {
		timerText = transform.Find ("PhaseSwitch").GetComponent<Text> ();
		roundText = transform.Find ("Round#").GetComponent<Text> ();
		p1ScoreText = transform.Find ("Player1Score").GetComponent<Text> ();
		p2ScoreText = transform.Find ("Player2Score").GetComponent<Text> ();
		infoText = transform.Find ("Info").GetComponent<Text> ();
		p1Role = transform.Find ("Player1Role").GetComponent<Image> ();
		p2Role = transform.Find ("Player2Role").GetComponent<Image> ();
		loserTextsP1 = transform.Find ("LoserTextsP1").gameObject;
		loserTextsP2 = transform.Find ("LoserTextsP2").gameObject;
		loserTextsP1.SetActive (false);
		loserTextsP2.SetActive (false);
		PlayerColorHolder ColorManager = GameObject.Find ("ColorHolder").GetComponent<PlayerColorHolder> ();
		Color temp1 = ColorManager.player1Color;
		Color temm2 = ColorManager.player2Color;

		color1 = new Color(temp1.r, temp1.g, temp1.b, 255);
		color2 = new Color(temm2.r, temm2.g, temm2.b, 255);

		/*
		Debug.Log ("Creating...");
		roleSprites = new SpriteRenderer[2];
		roleSprites [0] = new SpriteRenderer (); //gameObject.GetComponent<SpriteRenderer> ();
		roleSprites [0].sprite = roles [0];
		roleSprites [0].color = GameObject.Find ("Game").GetComponent<GC2> ().player1Color;
		roleSprites [1] = new SpriteRenderer (); //gameObject.GetComponent<SpriteRenderer> ();
		roleSprites [1].sprite = roles [1];
		roleSprites [1].color = GameObject.Find ("Game").GetComponent<GC2> ().player2Color;
		Debug.Log ("Finished...");
		*/
	}
	
	public void updateScoreboardAll(
		string timer, 
		int p1Score, 
		int p2Score, 
		int currPlayer, 
		int currCreator, 
		string round, 
		string info)
	{
		timerText.text = timer;
		roundText.text = round;
		//scores need to be flopped?
		p1ScoreText.text = p2Score.ToString ();
		p2ScoreText.text = p1Score.ToString ();
		if (currPlayer == 0) {
			p1Role.sprite = roles[0];
			p2Role.sprite = roles[1];
			p1Role.color = color1;
			p2Role.color = color2;
			p1ScoreText.color = color1;
			p2ScoreText.color = color2;
		} else {
			p1Role.sprite = roles[1];
			p2Role.sprite = roles[0];
			p1Role.color = color2;
			p2Role.color = color1;
			p1ScoreText.color = color2;
			p2ScoreText.color = color1;
		}

		infoText.text = info;
	}

	public void setLoser(int player)
	{
		switch (player) {
		case 0:
				loserTextsP1.SetActive (true);
				break;
		case 1:
				loserTextsP2.SetActive (true);
				break;
		default:
				loserTextsP1.SetActive (true);
				loserTextsP2.SetActive (true);
				break;
		}
	}

	public void updateScoreboardMessage(string timerMsg)
	{
		timerText.text = timerMsg;
	}
}
