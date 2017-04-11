using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameDebugController : MonoBehaviour
{
	public enum Phase {Player, Creator};
	public Phase phase;
	public bool useMoneyFromMap = false;
	public bool shouldGenerateMap = false;
	public string mapToGenerate = "Map0";

	public GameObject creatorPrefab;
	public GameObject playerPrefab;

	private Transform creatorContainer;
	private CreatorController creator;
	private CreatorHud creatorUI;

	private Transform playerContainer;
	private PlayerController player;
	private PlayerHud playerUI;

	public GameObject spawnedContainer;

	private GameObject mapContainer;
	private MapInfo mapinfo;

	new private DynamicCamera camera;
	//New keywords is used to hide the default Unity camera keyword for this one.

	void Start()
	{
		spawnedContainer = transform.FindChild("spawnedContainer").gameObject;
		camera = GameObject.Find("Main Camera").GetComponent<DynamicCamera>();
	}

	void Update() {

		if (Input.GetButtonDown("Back_1"))
			SceneManager.LoadScene("FinalGame");

		if (!mapinfo) {
			mapinfo = GameObject.FindObjectOfType<MapInfo> ();
			mapContainer = mapinfo.gameObject;

			// Read the level colors
			camera.GetComponent<Camera>().backgroundColor = mapinfo.backColor;
			camera.transform.Find("Back").GetComponentInChildren<ParticleSystem> ().startColor = mapinfo.particleColor1;
			camera.transform.Find("Back2").GetComponentInChildren<ParticleSystem> ().startColor = mapinfo.particleColor2;
		}

		if (!mapContainer && shouldGenerateMap)
		{
			generateMap(mapToGenerate);
		}

		if (phase == Phase.Creator)
		{
			//Remove the player
			if (playerContainer)
			{
				Destroy (playerContainer.gameObject);
			}
			//Create the Creator
			if (!creatorContainer)
			{
				createCreator ();
				if (mapinfo)
				{
					// Position creator at start
					Vector3 tempPos = mapinfo.startLocation.transform.position;
					tempPos.z = creator.transform.position.z;
					creator.transform.position = tempPos;
					if(useMoneyFromMap)
						creator.money = mapinfo.mapMoney;
					else
						creator.money = 999999999;
				}
				else
				{
					creator.money = 999999999;
				}
				creator.ui.updateMoneyText (creator.money);
			}
			camera.setFollowing(creator.gameObject);
		}
		if (phase == Phase.Player)
		{
			//Remove the Creator
			if (creatorContainer)
			{
				Destroy (creatorContainer.gameObject);
			}
			//Create the Player
			if (!playerContainer)
			{
				createPlayer ();
				if (mapinfo)
				{
					// Position player at start
					Vector3 tempPos = mapinfo.startLocation.transform.position;
					tempPos.z = player.transform.position.z;
					player.transform.position = tempPos;
				}
			}
			camera.setFollowing(player.gameObject);
			SentryController[] sentries = spawnedContainer.GetComponentsInChildren<SentryController>();
			foreach (SentryController sentry in sentries)
			{
				sentry.enabled = true;
				sentry.setPlayer();
			}
		}
	} 
	//End of update Method

	private void createPlayer()
	{
		playerContainer = Instantiate(playerPrefab).transform;
		player = playerContainer.Find("PlayerEnt").GetComponent<PlayerController>();
		playerUI = playerContainer.Find("PlayerUI").GetComponent<PlayerHud>();
		camera.setFollowing(player.gameObject);

		if (Input.GetJoystickNames().Length > 1)
		{
			player.setController(1);
		}
		else
		{
			player.setController(1);
		}
	}

	private void createCreator()
	{
		creatorContainer = Instantiate(creatorPrefab).transform;
		creator = creatorContainer.Find("CreatorEnt").GetComponent<CreatorController>();
		creatorUI = creatorContainer.Find("CreatorUI").GetComponent<CreatorHud>();
		camera.setFollowing(creator.gameObject);

		if (Input.GetJoystickNames().Length > 1)
		{
			creator.setController(2);
		}
		else
		{
			creator.setController(1);
		}
	}

	private void clearSpawnedObjects()
	{
		foreach (Transform spawned in spawnedContainer.transform)
			Destroy(spawned.gameObject);
	}

	private void enablePowerUps()
	{
		foreach (Transform powerUp in mapContainer.transform.Find("PowerUps"))
			powerUp.gameObject.SetActive(true);
	}

	public void respawnPlayer()
	{
		player.resetHealthOfPlayer();

		// Position player at start
		if (mapinfo) {
			Vector3 tempPos = mapinfo.startLocation.transform.position;
			tempPos.z = player.transform.position.z;
			player.transform.position = tempPos;
		}
	}

	public void applyGameObject(GameObject child)
	{
		child.transform.SetParent(spawnedContainer.transform);
	}

	public void generateMap(string mapName)
	{
		string mapPath = "Maps/" + mapName;
		mapContainer = Instantiate(Resources.Load(mapPath, typeof(GameObject))) as GameObject;
		mapinfo = mapContainer.GetComponent<MapInfo>();
	}
}
