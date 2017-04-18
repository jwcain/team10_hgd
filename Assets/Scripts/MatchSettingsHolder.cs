using UnityEngine;
using System.Collections;

public class MatchSettingsHolder : MonoBehaviour {

	public bool useTwoControllers;
	public int rounds;
	public bool randomMaps;

	public Color player1Color;
	public Color player2Color;

	public string mapToLoad;

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad (transform.gameObject);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
