using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Scoreboard : MonoBehaviour {

	public Sprite[] roleSprites;

	private Text timerText;
	private Text roundText;
	private Text p1ScoreText;
	private Text p2ScoreText;
	private Text infoText;
	private Image p1Role;
	private Image p2Role;
	private GameObject loserTextsP1;
	private GameObject loserTextsP2;

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
		p1ScoreText.text = p1Score.ToString ();
		p2ScoreText.text = p2Score.ToString ();
		if (currPlayer == 0) {
			p1Role.sprite = roleSprites [0];
			p2Role.sprite = roleSprites [1];
		} else {
			p1Role.sprite = roleSprites [1];
			p2Role.sprite = roleSprites [0];
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
