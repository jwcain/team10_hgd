using UnityEngine;
using System.Collections;

public class PlayerProfileManager : MonoBehaviour {

	public Profile testProfile;
	public static Profiles currentProfiles;
	public static string PROFILE_SAVE_NAME = "PlayerProfiles";

	void Start() {
		//Check to make sure we dont overide any files.
		if (currentProfiles != null) {
			Debug.LogError("You have potentially multiple PlayerProfileManagers.");
			return;
		}
		//Try to Load allProfiles.
		//If there isnt an allProfiles saved
			//Create a new allProfiles 
			//Create a Default Profile
			//set currentProfiles to the new one
			//Save the new one
		//Else
			//assign it to the currentProfiles
		Profiles t = BinaryPersistence.Load<Profiles>(PROFILE_SAVE_NAME);
		if (t == null) {
			//Create new defualt profile
			Profile deflt = new Profile(0, "Player", Color.gray);

			//Create the new allProfiles
			Profiles n = new Profiles();
			n.uidStart = 2;
			n.profiles = new Profile[] {deflt};

			//Set it as the currentProfiles and save it
			currentProfiles = n;
			saveProfiles();
		}
		else {
			currentProfiles = t;
		}

		testProfile = currentProfiles.profiles[0];
	}


	private static void saveProfiles() {
		BinaryPersistence.Save<Profiles>(currentProfiles, PROFILE_SAVE_NAME);
	}
	void OnApplicationQuit() {
		//Save the List!
		saveProfiles();
	}

	public static void AddProfile(string name, Color color) {
		//Create the new profile
		Profile newProfile = new Profile((currentProfiles.uidStart++), name,color);
		//add it to the list

		//We have to create a new Profiles since it includes an array that will not automatically resize
		Profiles newProfiles = new Profiles();
		newProfiles.uidStart = currentProfiles.uidStart;
		newProfiles.profiles = new Profile[currentProfiles.profiles.Length + 1];
		//Copy over all the references to the old profiles
		for (int i = 0; i < currentProfiles.profiles.Length; i++) newProfiles.profiles[i] = currentProfiles.profiles[i];
		//Add the new one
		newProfiles.profiles[newProfiles.profiles.Length - 1] = newProfile;
		//Override the old list
		currentProfiles = newProfiles;

		//save the list!
		saveProfiles();
	}

	public static void RemoveProfile(Profile profile) {
		//We have to create a new Profiles since it includes an array that will not automatically resize
		Profiles newProfiles = new Profiles();
		newProfiles.uidStart = currentProfiles.uidStart;
		newProfiles.profiles = new Profile[currentProfiles.profiles.Length - 1];

		//Copy over all the references to the old profiles, except the one we intend of removing
		for (int i = 0; i < currentProfiles.profiles.Length; i++) {
			if (currentProfiles.profiles[i].Equals(profile)) continue; // Do not copy over the one we are removing
			newProfiles.profiles[i] = currentProfiles.profiles[i];

		}
		//override the old list
		currentProfiles = newProfiles;

		//save the list!
		saveProfiles();
	}


	public static Profile getProfileFromUID(int UID) {
		//Loop through all the profiles
		for (int i = 0; i < currentProfiles.profiles.Length; i++) {
			//Check if the uid's match
			if (currentProfiles.profiles[i].uid == UID)
				return currentProfiles.profiles[i];
		}
		//If we exited the loop here, that means we didnt find it
		return null;
	}

	[System.Serializable]
	public class Profiles {
		public int uidStart;
		public Profile[] profiles;
	}
}
