using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{

	private float countdownTimer;

	// To determine which controller is which player
	// This is for the purpose of swapping roles
	// public int whoIsPlayer;
	// public int whoIsCreator;

	public int maxRounds;

	private int[] scores = { 0, 0 };
	private int currPlayer;
	private int currCreator;
	//Increase this for a longer Creator phase
	private float timer = 10.0F;
	private int state;
	private int round;
	private bool ranTwice;
	private bool playerReachedEnd;

	private float width;
	private float startMaxXPos;
	public string[] phaseSwitchMessages = { "Time's Up!", "Get Ready...", "3", "2", "1", "Go!" };
	public float[] phaseSwitchTimes = { 1f, 2f, 0.5f, 0.5f, 0.5f, 0.5f };
	private int phaseSwitchState = 0;

	public GameObject scoreboardCanvas;
	private Scoreboard scoreboard;
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
		state = 0;
		round = 1;
		timer = 90f;
		scores[0] = 0;
		scores[1] = 0;
		currPlayer = 0;
		currCreator = 1;
		ranTwice = false;
		playerReachedEnd = false;

		if (maxRounds <= 0)
			maxRounds = 5;

		spawnedContainer = transform.FindChild("spawnedContainer").gameObject;
		camera = GameObject.Find("Main Camera").GetComponent<DynamicCamera>();
		scoreboardCanvas = Instantiate(scoreboardCanvas);
		scoreboard = scoreboardCanvas.transform.FindChild("Scoreboard").GetComponent<Scoreboard>();
		scoreboardCanvas.SetActive(false);
	}

	void Update() {

		if (Input.GetButtonDown("Back_1"))
			SceneManager.LoadScene("FinalGame");

		switch (state)
		{
		case 0: //Creator
			{
				if (!mapContainer)
				{
					generateMap();
				}
				
				string timeText;
				timeText = (int)((timer + 1) / 60) + ":" + (int)(((timer + 1) % 60) / 10) + (int)(((timer + 1) % 60) % 10);
				if (!creator)
				{
					createCreator();
					// Position creator at start
					Vector3 tempPos = mapinfo.startLocation.transform.position;
					tempPos.z = creator.transform.position.z;
					creator.transform.position = tempPos;
					creator.money = mapinfo.mapMoney;
					creator.ui.updateMoneyText(creator.money);
				}
				creator.ui.updateTimers(timeText);

				if (timer <= 0 || creator.money <= 0)
				{
					creatorContainer.gameObject.SetActive(false);
					scoreboardCanvas.SetActive(true);
					scoreboard.updateScoreboardAll(
						phaseSwitchMessages[0], 
						scores[0], 
						scores[1], 
						currPlayer, 
						currCreator, 
						round.ToString(),
						"Starting Player Phase");
					phaseSwitchState = 0;
					timer = phaseSwitchTimes[0];
					nextState();
				}
				break;
			}
		case 1: //Phase Switch
			{
				if (timer <= 0)
				{
					phaseSwitchState++;
					if (phaseSwitchState >= phaseSwitchMessages.Length)
					{
						scoreboardCanvas.SetActive(false);
						if (!playerContainer)
							createPlayer();
						playerContainer.gameObject.SetActive(true);
						camera.setFollowing(player.gameObject);

						timer = mapinfo.timeToFinish;

						// Position player at start
						Vector3 tempPos = mapinfo.startLocation.transform.position;
						tempPos.z = player.transform.position.z;
						player.transform.position = tempPos;

						SentryController[] sentries = spawnedContainer.GetComponentsInChildren<SentryController>();
						foreach (SentryController sentry in sentries)
						{
							sentry.enabled = true;
							sentry.setPlayer();
						}

						nextState();
					}
					else
					{
						scoreboard.updateScoreboardMessage(phaseSwitchMessages[phaseSwitchState]);
						timer = phaseSwitchTimes[phaseSwitchState];
					}
				}
				break;
			}
		case 2: //Player
			{
				string timeText;
				timeText = (int)((timer + 1) / 60) + ":" + (int)(((timer + 1) % 60) / 10) + (int)(((timer + 1) % 60) % 10);
				player.ui.updateTimers(timeText);
				if (timer <= 0 || playerReachedEnd)
				{
					playerContainer.gameObject.SetActive(false);

					int cPlayerScore;
					int cCreatorScore;
					if (playerReachedEnd)
					{
						cPlayerScore = (int)((timer / mapinfo.timeToFinish) * 1000);
						cCreatorScore = 1000 - cPlayerScore;
					}
					else
					{
						cPlayerScore = 100;
						cCreatorScore = 400;
					}
					scores[currPlayer] += cPlayerScore;
					scores[currCreator] += cCreatorScore;

					// Swap the roles
					if (currPlayer == 0 && currCreator == 1)
					{
						currPlayer = 1;
						currCreator = 0;
					}
					else
					{
						currPlayer = 0;
						currCreator = 1;
					}
					player.setController(currPlayer + 1);
					creator.setController(currCreator + 1);

					// The string to Print above the scoreboard
					string information;

					if (ranTwice)
					{
						round++;
						ranTwice = false;
						information = "Starting Next Round";
						Destroy(mapContainer);
					}
					else
					{
						information = "Swapping Roles";
						ranTwice = true;
					}

					if (round > maxRounds)
					{
						information = "The Loser Is...";
						state = 4;
						scoreboardCanvas.SetActive(true);
						scoreboard.updateScoreboardAll(
							phaseSwitchMessages[0], 
							scores[0], 
							scores[1], 
							currPlayer, 
							currCreator, 
							round.ToString(),
							information);
						break;
					}

					scoreboardCanvas.SetActive(true);
					scoreboard.updateScoreboardAll(
						phaseSwitchMessages[0], 
						scores[0], 
						scores[1], 
						currPlayer, 
						currCreator, 
						round + "\\" + maxRounds,
						information);

					// How long to wait for swap phase
					timer = 10f;

					// Reset the player to starting
					player.resetEverything();
					nextState();
					playerReachedEnd = false;
				}
				else if (player.currentHealth <= 0)
				{
					player.resetHealthOfPlayer();

					// Position player at start
					Vector3 tempPos = mapinfo.startLocation.transform.position;
					tempPos.z = player.transform.position.z;
					player.transform.position = tempPos;
					scores[currPlayer] -= 100;
					scores[currCreator] += 100;
				}
				break;
			}
		case 3:
			{
				clearSpawnedObjects();
				string timeText;
				timeText = (int)((timer + 1) / 60) + ":" + (int)(((timer + 1) % 60) / 10) + (int)(((timer + 1) % 60) % 10);
				scoreboard.updateScoreboardMessage(timeText);

				creator.money = mapinfo.mapMoney;
				creator.ui.updateMoneyText(mapinfo.mapMoney);

				if (timer <= 0)
				{
					timer = 90f;
					scoreboardCanvas.gameObject.SetActive(false);
					creatorContainer.gameObject.SetActive(true);

					if (!mapContainer)
						generateMap();

					enablePowerUps();

					// Position creator at start
					Vector3 tempPos = mapinfo.startLocation.transform.position;
					tempPos.z = creator.transform.position.z;
					creator.transform.position = tempPos;

					camera.setFollowing(creator.gameObject);
					nextState();
				}
				break;
			}
		case 4: // END GAME
			{
				if (scores[0] < scores[1])
					scoreboard.setLoser(0);
				else if (scores[1] < scores[0])
					scoreboard.setLoser(1);
				else
					scoreboard.setLoser(3);
				
				if (Input.GetButtonDown("A_1") || Input.GetButtonDown("A_2"))
				{
					SceneManager.LoadScene("MainMenu");	
				}
				break;
			}
		}



		timer -= Time.deltaTime;
	} 
	//End of update Method

	private void nextState()
	{
		if (state >= 3)
			state = 0;
		else
			state = state + 1;
	}

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

	public void applyGameObject(GameObject child)
	{
		child.transform.SetParent(spawnedContainer.transform);
	}

	public void generateMap()
	{
		string rnd = Random.Range(1, 4).ToString();
		string mapPath = "Map" + rnd;
		mapContainer = Instantiate(Resources.Load(mapPath, typeof(GameObject))) as GameObject;
		mapinfo = mapContainer.GetComponent<MapInfo>();
	}

	public void endPlayerPhase()
	{
		playerReachedEnd = true;
	}
}
