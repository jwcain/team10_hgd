using UnityEngine;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class BinaryPersistance : MonoBehaviour {

	/* In order to use this properly, you need an intermediary class to hold your data
	 * This class has to have the [System.Serializable] tag and try to make it as little
	 * data intensive as possible, but it probably wont matter to much.
	 * Here is an example class covering some basic data types we might want to store. 
	 * 
		[System.Serializable]
		class simpleSave {
			public float a;
			public string b;
			public int[] c;
			public bool d;
		}
	 *	Continuing with the example, we would create a simpleSave class
			simpleSave obj = new SimpleSave();
	 *	Then add data
			obj.a = 1.0;
			obj.b = "test"
			etc.
 	 *
	 *	Then we call 
			BinaryPersistance.Save<simpleSave>(obj, "savename");
	 *	this will compile and save a binary savename.dat file in the Application.persistantDataPath folder.
	 *	Typically this is somewhere in user/AppData
	 *	To load, simply call
	 		simpleSave obj = BinaryPersistance.Load<simpleSave>("savename");
	 *	This will look for and open the savename.dat file and compile it back into an object
	 *	NOTE: this does not check if the generic type you pass to the method is correct for the file you are opening, so be carefull.
	 *	You will get a simpleSave instance that contains all of your previous data.	
	 * I hope this explanation was helpful
	 * - Justin
	 */



	public static void Save<T>(T obj, string saveName) {
		//So for binary formating the object we are saving has to be serializable,
		// Ususally this means it has to be tagged byt the [System.Serializable] tag abover the 
		// class. 

		if (obj.GetType().IsSerializable == false) {
			Debug.LogError("Warning: Object is not serializable and therefore could not be saved.");
			return; // return for an early exit
		}

		//Open a binary formatter
		BinaryFormatter bf = new BinaryFormatter();

		//Put together the file path for our new file.
		string path = Application.persistentDataPath;
		path += "/" + saveName + ".dat";

		//Opens the file. This will create it if it does not exist, or just open it to
		// be overwritten otherwise.

		try {
			FileStream file = File.Open(path, FileMode.OpenOrCreate);

			//Serialize the object into the file!
			bf.Serialize(file, obj);

			file.Flush();
			file.Close();
		}
		catch {
			Debug.LogError("Unable to save " + saveName + ".");
			return;
		}
	}

	public static T Load<T>(string saveName) {
		
		//Open a binary formatter
		BinaryFormatter bf = new BinaryFormatter();

		//Put together the file path for our new file.
		string path = Application.persistentDataPath;
		path += "/" + saveName + ".dat";

		try {
			FileStream file = File.Open(path,FileMode.Open);
			T toReturn = (T)bf.Deserialize(file);
			//[TODO] Potentially validate that we loaded non garbage data?
			file.Flush();
			file.Close();
			return toReturn;
		} 
		catch {
			Debug.LogError("Unable to load " + saveName + " may not exist.");
			//Debug.LogError();
			return default(T);
		}
	}
}
