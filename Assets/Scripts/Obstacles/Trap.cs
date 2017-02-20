using UnityEngine;
using System.Collections;

public class Trap : MonoBehaviour {

	public string name; // [TODO] this might need to be changed, it is currently hiding the defualt unity name keyword for gameobjects
	public string description; // Maybe delete
	public int cost;
	public bool canPlaceOnWalls;
	public bool canPlaceInAir;

}
