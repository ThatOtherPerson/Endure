﻿using UnityEngine;
using System.Collections;

public class WorldController : MonoBehaviour {

	public static WorldController instance = null;
	public int size = 3;

	// TODO: the RoomManager should probably hold this
	public GameObject[] rooms;

	private RoomManager roomScript;

	public GameObject[] roomTypes;

	private PlayerController player;
	private CameraController camera;

	void Awake () {
//		if (instance == null) {
//			instance = this;
//		} else if (instance != this) {
//			Destroy(gameObject);
//			DontDestroyOnLoad(gameObject);
			roomScript = GetComponent<RoomManager>();
			InitGame();
//		}
	}

	void InitGame () {
		roomScript.SetupRoom ();
	}

	// TODO: make better
	private bool cameraLock = false;
	
	private float roomWidth;
	private float roomHeight;

	private float playerWidth;
	private float playerHeight;

	private GameObject[,] rooms;
	private int roomX = 0;
	private int roomY = 0;

	// Use this for initialization
	void Start () {
		this.player = PlayerController.instance;
		this.camera = CameraController.instance;

//		GameObject nextRoom = rooms[0];
//		float width = nextRoom.GetComponent<SpriteRenderer>().sprite.bounds.size.x;
//		float scale = (float) nextRoom.transform.localScale.x;		
//		for (int i = 0; i < this.size; i++) {
//			for (int j = 0; j < this.size; j++){
//				Instantiate (rooms[0], new Vector3(i*width*scale, j*width*scale, 0), Quaternion.identity);
//			}
//		}
		
		// TODO: actually calculate this
		this.roomWidth = 32;
		this.roomHeight = 32;

		// TODO: view note about this.rooms at top
		this.rooms = new GameObject[this.size, this.size];
	}
	
	// Update is called once per frame
	void Update () {
		this.TrackPlayer ();
		this.CheckForRoomChange ();
	}

	void TrackPlayer () {
		float playerX = this.player.transform.position.x;
		float playerY = this.player.transform.position.y;

		GameObject targetRoom = this.rooms [this.roomX, this.roomY];

		float inRoomX = playerX - targetRoom.transform.position.x;
		float inRoomY = playerY - targetRoom.transform.position.y;

		// make camera track player

		float newCameraX = playerX;
		float newCameraY = playerY;

		// only move camera to match player if the camera isn't at room edges

		newCameraX = targetRoom.transform.position.x + this.Restrain (inRoomX, this.camera.getWidth (), this.roomWidth);
		newCameraY = targetRoom.transform.position.y + this.Restrain (inRoomY, this.camera.getHeight (), this.roomHeight);

		this.camera.transform.position = new Vector3 (newCameraX,
		                                              newCameraY,
		                                              this.camera.transform.position.z);
	}

	void CheckForRoomChange () {
		float playerX = this.player.transform.position.x;
		float playerY = this.player.transform.position.y;

		GameObject targetRoom = this.rooms [this.roomX, this.roomY];

		this.playerWidth = this.player.GetComponent<SpriteRenderer>().sprite.bounds.size.x * this.player.transform.localScale.x;
		this.playerHeight = this.player.GetComponent<SpriteRenderer>().sprite.bounds.size.y * this.player.transform.localScale.y;

		// detect when player hits boundary
		if (this.roomWidth / 2 + targetRoom.transform.position.x - playerX < this.playerWidth / 2) {
			// east
			this.ChangeRoom (1, 0);
		} else if (this.roomWidth / 2 + targetRoom.transform.position.x + playerX < this.playerWidth / 2) {
			// west
			this.ChangeRoom (-1, 0);
		} else if (this.roomHeight / 2 + targetRoom.transform.position.y - playerY < this.playerHeight / 2) {
			// north
			this.ChangeRoom (0, 1);
		} else if (this.roomHeight / 2 + targetRoom.transform.position.y + playerY < this.playerHeight / 2) {
			// south
			this.ChangeRoom (0, -1);
		}
	}

	void ChangeRoom (int x, int y)
	{
		this.roomX += x;
		this.roomY += y;

		// TODO: get rid of ChangeRoom, just change roomX and roomY, then make
		// TrackPlayer use roomX and roomY to adjust the camera so it's not out-of-bounds
		// after setting it equal to the player

		this.player.transform.Translate (new Vector2 (this.playerWidth * x * 1.1f, this.playerHeight * y * 1.1f));

		Vector2 targetTranslation = new Vector2(this.camera.getWidth () * x, this.camera.getHeight () * y);
		this.camera.transform.Translate (targetTranslation);
		//this.player.transform.position = new Vector2 (0, 0);
	}

	bool WithinOffset (float coordinate, float offset) {
		return coordinate <= offset - (offset / 2) && coordinate >= -offset / 2;
	}

	float Restrain (float coordinate, float cameraWidth, float roomWidth)
	{
		if (coordinate + cameraWidth / 2 > roomWidth / 2) {
			return roomWidth / 2 - cameraWidth / 2;
		} else if (coordinate - cameraWidth / 2 < -roomWidth / 2) {
			return -roomWidth / 2 + cameraWidth / 2;
		}

		return coordinate;
	}
}
