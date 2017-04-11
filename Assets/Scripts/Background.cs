using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Background : MonoBehaviour {

	Camera cam;
	ParticleSystem ps;

	void Awake () {
		this.cam = Camera.main;
		this.ps = GetComponent<ParticleSystem> ();
	}
	
	// Update is called once per frame
	void Update () {
		// Make the particle space twice the size of the camera
		float height = 4f * cam.orthographicSize;
		float width = height * cam.aspect;
		transform.localScale = new Vector3 (width, height, 1);
	}
}
