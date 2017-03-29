using UnityEngine;

[System.Serializable]
public class Profile {
	public int uid;
	public string name;
	public int[] highscores;
	public Color characterColor;

	public Profile(int UID, string name, Color color) {
		this.uid = UID;
		this.name = name;
		this.characterColor = color;
		this.highscores = new int[10];
	}

	public void AddHighscore(int score) {
		// loop through the array, find the spot where it fits, add it.
		for (int i = 0; i < highscores.Length; i++) {
			if (score > highscores[i]) {
				//Shift everything right, put this here.
				for (int j = i; j < highscores.Length - 1; j++) {
					highscores[j + 1] = highscores[j];
				}
				highscores[i] = score;
				break;
			}
		}
	}
}