using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LeaderboardGUI : MonoBehaviour {

	private GUIStyle titleStyle = new GUIStyle();
	private GUIStyle lbStyle = new GUIStyle();
	public Font f;

	private void OnGUI() {

		// Set styles:
		titleStyle.fontSize = 28;
		titleStyle.font = f;
		titleStyle.normal.textColor = Color.black;
		titleStyle.alignment = TextAnchor.MiddleCenter;

		lbStyle.fontSize = 20;
		lbStyle.font = f;
		lbStyle.normal.textColor = Color.red;

		Color myColor = new Color ();
		ColorUtility.TryParseHtmlString ("#00B5FFFF", out myColor);
		lbStyle.normal.textColor = myColor;

		// Create GUI with leaderboard:
		GUILayout.BeginArea (new Rect(0, 0, Screen.width, Screen.height-100));
		GUILayout.Label ("\n\nTry and Stop Me Leaderboard\n", titleStyle);
		GUILayout.Label ("\t\t\t  Rank:\t\t  Name:\t\t  Score:\n", lbStyle);
		for(int ctr = 0; ctr < Leaderboard.numHighScores; ++ctr) {
			var hs = Leaderboard.getHS (ctr);
			GUILayout.Label ("\t\t\t       " + (ctr + 1).ToString() + ".\t\t   " +  hs.name + "\t\t    " + hs.score, lbStyle);
		}
		GUILayout.EndArea ();
	}

}