using UnityEngine;
using System.Collections;

public class PlayerColorHolder : MonoBehaviour {

	public Color player1Color;
	public Color player2Color;

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad (transform.gameObject);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
