using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Leaderboard {

	public const int numHighScores = 10;

	public struct HighScore {
		public int score;
		public string name;

		public HighScore(int score, string name) {
			this.score = score;
			this.name = name;
		}
	}

	public static List<HighScore> lbScores;
	private static List<HighScore> scores {
		get {
			if (lbScores == null) {
				lbScores = new List<HighScore> ();
				loadLB ();
			}
			return lbScores;
		}
	}

	private static void sortLB() {
		lbScores.Sort ((x, y) => y.score.CompareTo(x.score));
	}

	private static void loadLB() {
		lbScores.Clear ();
		for (int ctr = 0; ctr < numHighScores; ++ctr) {
			HighScore hs;
			hs.score = PlayerPrefs.GetInt("lb[" + ctr + "].score", 0);
			hs.name = PlayerPrefs.GetString("lb[" + ctr + "].name", "");
			lbScores.Add(hs);
		}
		sortLB();
	}

	private static void saveLB() {
		for (int ctr = 0; ctr < numHighScores; ++ctr) {
			var hs = lbScores [ctr];
			PlayerPrefs.SetInt ("lb[" + ctr + "].score", hs.score);
			PlayerPrefs.SetString ("lb[" + ctr + "].name", hs.name);
		}
	}

	public static HighScore getHS(int i) {
		return scores [i];
	}

	public static int setHS(int score, string name) {
		int ret = 0;

		// Add new score and sort:
		scores.Add(new HighScore(score, name));
		sortLB ();

		// Change return if score is in top 10:
		if (!getHS(scores.Count - 1).name.Equals ("YOU")) {
			ret = 1;
		}

		// Remove scored outside top 10, save them and return:
		scores.RemoveAt (scores.Count - 1);
		saveLB ();
		return ret;
	}
}
