using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class MultiplayerInputMapper : NetworkBehaviour {

	public override void OnStartLocalPlayer() {
		this.gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
	}

	// Update is called once per frame
	void Update () {
		//Do not run this method on this client if it is not our local player object
		if (!isLocalPlayer) {
			return;
		}

		string button = "Test";
		if (Input.GetButtonDown(button)) {
			Debug.Log("Click");
			Vector3 rng = new Vector3(Random.value, Random.value, Random.value);
			this.transform.position = rng;
		}

	}
}
