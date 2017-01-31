using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {

	static MultiplayerInputMapper player1;
	static MultiplayerInputMapper player2;

	//Returns true if the requesting client was able to register a new controller for the game
	static bool RegisterController(MultiplayerInputMapper controller) {
		if (player1 == null) {
			player1 = controller;
			return true;
		}
		else if (player2 == null) {
			player2 = controller;
			return true;

		}

		return false;
	}
	// Update is called once per frame
	void Update () {
	
	}
}
