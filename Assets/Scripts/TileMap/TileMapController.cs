using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using Lean;
using LitJson;

public enum Tiles {
	// TODO: Update these...
	Unset,
	Ground,
	Spike,
	Water,
	Rainbow
}

[Serializable]
public class TileSprite {
	public string tName;
	public Sprite tImage;
	public Tiles tType;

	public TileSprite() {
		tName = "Unset";
		tImage = new Sprite ();
		tType = Tiles.Unset;
	}

	public TileSprite(string name, Sprite image, Tiles tile)
	{
		this.tName = name;
		this.tImage = image;
		this.tType = tile;
	}
}

public class TileMapController : MonoBehaviour {

	public List<TileSprite> tileSprites;
	public Vector2 mapSize;
	public Sprite defaultImage;
	public GameObject tileContainerPrefab;
	public GameObject tilePrefab;
	public GameObject toFollow;

	private Vector2 currentPosition;
	private TileSprite[,] _map;
	private GameObject controller;
	private GameObject _tileContainer;
	private List<GameObject> _tiles = new List<GameObject>();

	private string jsonString;
	private JsonData objJson;

	private JsonData generateJsonObject (string fname)
	{
		jsonString = File.ReadAllText(Application.dataPath + "/JSON_files/" + fname);
		Debug.Log (jsonString);
		objJson = JsonMapper.ToObject (jsonString);

		return objJson;
	}

	private TileSprite findTile(Tiles tile)
	{
		foreach (TileSprite tileSprite in tileSprites) {
			if (tileSprite.tType == tile)
				return tileSprite;
		}
		return null;
	}

	// Used to create default tiles
	// Not used currently cause I only want tiles that the map
	//	wants has specified to spawn
	private void defaultTiles() 
	{
		for (int y = 0; y < mapSize.y; y++) {
			for (int x = 0; x < mapSize.x; x++) {
				_map [x, y] = new TileSprite ("Unset", defaultImage, Tiles.Unset);
			}
		}
	}

	private void generateTiles(int x, int y, int tID)
	{
		TileSprite tSprite = new TileSprite (tileSprites[tID].tName, 
											 tileSprites[tID].tImage, 
											 tileSprites[tID].tType);
		_map [x, (int)mapSize.y - y] = tSprite;
	}

	private void setTiles(JsonData map)
	{
		int index = 0;
		for (int y = 0; y < mapSize.y; y++) {
			for (int x = 0; x < mapSize.x; x++) {
				int tileID = (int) map ["layers"] [0] ["data"] [index];
				if (tileID != 0)
					generateTiles (x, y, tileID);
				index++;
			}
		}
	}

	private void addTilesToWorld()
	{
		foreach (GameObject o in _tiles) {
			LeanPool.Despawn (o);
		}
		_tiles.Clear ();
		LeanPool.Despawn(_tileContainer);
		_tileContainer = LeanPool.Spawn (tileContainerPrefab);

		for (float y = 0; y < mapSize.y; y++) {
			for (float x = 0; x < mapSize.x; x++) {
				if (_map [(int)x, (int)y] == null)
					continue;
				GameObject t = LeanPool.Spawn (tilePrefab);
				t.transform.position = new Vector3 (x, y, -8);
				t.transform.SetParent (_tileContainer.transform);
				SpriteRenderer renderer = t.GetComponent<SpriteRenderer> ();
				renderer.sprite = _map [(int)x , (int)y].tImage;
				_tiles.Add (t);
			}
		}

		// If you want to procedurally generate the tilemap as the character moves
		// Kind of weird and doesnt seem smooth
		// So taking it out for now... Maybe we might need in the future?
		/*
		foreach (GameObject o in _tiles) {
			LeanPool.Despawn (o);
		}
		_tiles.Clear ();
		LeanPool.Despawn(_tileContainer);
		_tileContainer = LeanPool.Spawn (tileContainerPrefab);
		float tileSize = 0.64F;
		float viewOffsetX = viewPortSize.x / 2F;
		float viewOffsetY = viewPortSize.y / 2F;
		
		for (float y = -viewOffsetY; y < viewOffsetY; y++) {
			for (float x = -viewOffsetX; x < viewOffsetX; x++) {
				float tX = x+tileSize;
				float tY = y+tileSize;

				float iX = x + currentPosition.x;
				float iY = y + currentPosition.y;

				// Make sure that we do not go out of bounds
				if (iX < 0)
					continue;
				if (iY < 0)
					continue;
				if (iX > mapSize.x - 2)
					continue;
				if (iY > mapSize.y - 2)
					continue;
				
				GameObject t = LeanPool.Spawn (tilePrefab);
				t.transform.position = new Vector3 (tX, tY, -8);
				t.transform.SetParent (_tileContainer.transform);
				SpriteRenderer renderer = t.GetComponent<SpriteRenderer> ();
				renderer.sprite = _map [(int)x + (int)currentPosition.x, (int)y + (int)currentPosition.y].tImage;
				_tiles.Add (t);
			}
		}
		*/
	}

	public void Start()
	{
		// controller = GameObject.Find ("TileMapController");
		// Camera cam = GameObject.Find ("Main Camera").GetComponent<Camera> ();
		// float height = 2f * cam.orthographicSize;
		// float width = height * cam.aspect;

		_map = new TileSprite[(int) mapSize.x, (int) mapSize.y];

		JsonData map = generateJsonObject ("test_2.json");

		setTiles (map);
		addTilesToWorld ();
	}

	private void Update()
	{}
}
