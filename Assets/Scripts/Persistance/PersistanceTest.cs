using UnityEngine;
using System.Collections;

public class PersistanceTest : MonoBehaviour {

	//Different types to test
	public float a = 0.0f;
	public string b = "Default";
	public int[] c = new int[3];
	public bool d = false;

	public bool save;
	public bool load;

	void Start() {
		Debug.Log(Application.persistentDataPath);
	}
	
	// Update is called once per frame
	void Update () {
		if (save == true) {
			simpleNumSave obj = new simpleNumSave();
			obj.a = a;
			obj.b = b;
			obj.c = c;
			obj.d = d;

			BinaryPersistance.Save<simpleNumSave>(obj, "bintest");
			save = false;
		}

		if (load == true) {
			simpleNumSave obj = BinaryPersistance.Load<simpleNumSave>("bintest");
			a = obj.a;
			b = obj.b;
			c = obj.c;
			d = obj.d;

			load = false;
		}
	}

	[System.Serializable]
	class simpleNumSave {
		public float a;
		public string b;
		public int[] c;
		public bool d;
	}
}
