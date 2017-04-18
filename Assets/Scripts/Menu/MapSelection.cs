using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MapSelection : MonoBehaviour {

	GameObject canvas;
	Image image;
	Text mapText;
	GameObject knob;
	float knobWidth;

	int selected;
	bool canInteract;

	List<MapInfo> maps;
	List<GameObject> knobs;

	public bool done;

	// Use this for initialization
	void Start () {
		canvas = GameObject.Find ("MapSelectionCanvas");
		image = GameObject.Find ("Image").GetComponent<Image>();
		mapText = GameObject.Find ("MapText").GetComponent<Text>();
		knob = GameObject.Find ("Knob");
		knobWidth = knob.GetComponent<RectTransform> ().rect.width;

		selected = 0;
		canInteract = true;

		//Load all the maps
		maps = new List<MapInfo>();
		Object[] mapOs = Resources.LoadAll ("Maps/");
		foreach(Object o in mapOs) {
			GameObject mapGo = (GameObject)o;
			if (mapGo.GetComponent<MapInfo> () && mapGo.name.ToCharArray () [0] != '~') {
				maps.Add (mapGo.GetComponent<MapInfo> ());
			}
		}

		//Create the knobs at the bottom
		knobs = new List<GameObject>();
		for (int i = 0; i < maps.Count; i++) {
			GameObject newKnob = (GameObject)GameObject.Instantiate (knob, canvas.transform);
			float newX = (i * knobWidth);
			newKnob.transform.localPosition = new Vector3 (newX, newKnob.transform.localPosition.y, newKnob.transform.localPosition.z);
			newKnob.GetComponent<RawImage> ().color = maps [i].backColor;
			if (i == 0) {
				newKnob.transform.localScale = new Vector3 (1.5f, 1.5f, 1);
			}
			knobs.Add (newKnob);
		}

		done = false;
	}
	
	// Update is called once per frame
	void Update () {
		float xOne = Input.GetAxisRaw ("L_XAxis_1");
		float xTwo = Input.GetAxisRaw ("L_XAxis_2");
		if ((Mathf.Abs(xOne) > 0.1f || Mathf.Abs(xTwo) > 0.1f) && canInteract) {
			canInteract = false;
			StartCoroutine (SelectionChange (xOne, xTwo));
		}
		if (Input.GetButtonDown ("A_1")) {
			GameObject.Find ("SettingsHolder").GetComponent<MatchSettingsHolder> ().mapToLoad = maps [selected].name;
			if(GameObject.Find("Game"))
				done = true;
			else
				SceneManager.LoadScene ("FinalGame");
		}
	}

	IEnumerator SelectionChange(float xOne, float xTwo)
	{
		if (xOne > 0.1f || xTwo > 0.1f) {
			//move right
			if (selected < maps.Count - 1) {
				selected++;
				mapText.text = "< " + maps [selected].name + " >";

				foreach (GameObject go in knobs) {
					Vector3 pos = go.transform.localPosition;
					go.transform.localPosition = new Vector3 (pos.x - knobWidth, pos.y, pos.z);

					if (go.transform.localPosition.x == 0) {
						go.transform.localScale = new Vector3 (1.5f, 1.5f, 1);
					} else {
						go.transform.localScale = new Vector3 (1, 1, 1);
					}
				}
			}
		} else if (xOne < -0.1f || xTwo < -0.1f) {
			//move left
			if (selected > 0) {
				selected--;
				mapText.text = "< "  + maps [selected].name + " >";

				foreach (GameObject go in knobs) {
					Vector3 pos = go.transform.localPosition;
					go.transform.localPosition = new Vector3 (pos.x + knobWidth, pos.y, pos.z);

					if (go.transform.localPosition.x == 0) {
						go.transform.localScale = new Vector3 (1.5f, 1.5f, 1);
					} else {
						go.transform.localScale = new Vector3 (1, 1, 1);
					}
				}
			}
		}

		yield return new WaitForSeconds (0.2f);
		canInteract = true;
	}
}
