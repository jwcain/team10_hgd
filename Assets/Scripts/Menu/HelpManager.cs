using UnityEngine;
using System.Collections;

public class HelpManager : MonoBehaviour {
	public Transform optionsMenu;
	public UnityEngine.EventSystems.EventSystem events;

	// Use this for initialization
	void Start () {
		Time.timeScale = 1;
	}

	public void ControlsMenu(bool clicked){
		//switch controls for help or controls menu
		optionsMenu.FindChild("Help Text").gameObject.SetActive(!clicked);
		optionsMenu.FindChild("Help").gameObject.SetActive(clicked);
		optionsMenu.FindChild("Controls Text").gameObject.SetActive(clicked);
		optionsMenu.FindChild("Controls").gameObject.SetActive(!clicked);
		//change some other stuff
		if (clicked = true) {
			//set selected button for controller support
			events.SetSelectedGameObject (optionsMenu.FindChild ("Back").gameObject);				
		} else {
			events.SetSelectedGameObject (optionsMenu.FindChild ("Back").gameObject);	
		}
	}
}
