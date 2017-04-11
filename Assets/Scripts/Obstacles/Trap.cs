using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Trap : MonoBehaviour {

	public string name; // [TODO] this might need to be changed, it is currently hiding the defualt unity name keyword for gameobjects
	public string description; // Maybe delete
	public int cost;
	public bool canPlaceOnWalls;
	public bool canPlaceInAir;

	public MonoBehaviour[] activateTheseScriptsOnRunnersTurn;

	private static List<Trap> activeTraps;


	public static void ActivateDelayedTraps() {
		foreach (Trap t in activeTraps) {
			foreach (MonoBehaviour s in t.activateTheseScriptsOnRunnersTurn) {
				s.enabled = true;
			}
		}
	}

	void Start() {
		if (activeTraps == null) {
			activeTraps = new List<Trap>();
		}
		activeTraps.Add(this);
	}

}
