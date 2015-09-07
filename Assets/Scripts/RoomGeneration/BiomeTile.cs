using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class BiomeTile : MonoBehaviour
{
	public GameObject[] groundTiles;
	public GameObject[] blockingTiles;
	public GameObject[] enemies;

	protected Tile[,] tileMap;
	protected int width;
	protected int height;
	
	void Awake() {
		this.tileMap = this.GetComponent<RoomManager>().tileMap;
		this.height = this.tileMap.GetLength(0);
		this.width = this.tileMap.GetLength(1);
	}

	public delegate void TilePlacer(int x, int y);
	// Blooms are great for making irregular, but roundish shapes liek bodies of water
	public void BlockingExplosion(int x, int y, int level, TilePlacer spritePlacer) {

		if (Random.Range (0, level) < 1 || x < 0 || y < 0 || x >= this.width || y >= this.height) {
			return;
		}
		
		Tile tile = this.tileMap[x, y];
		
		if (tile.biome != this.getBiomeNumber() || tile.blocking || tile.path) {
			return;
		}
		
		if (tile.item == null) {
			tile.blocking = true;
			spritePlacer(x, y);
		}
		
		for (int i = -1; i <= 1; i++) {
			for (int j = -1; j <= 1; j++) {
				if (Random.Range(0, 10) > 3 && (j != 0 || i == 1)) {
					BlockingExplosion(x + i, y + j, level - 1, spritePlacer);
				}
			}
		}
	}

	// Perlin is faster than blooms and creates a maze-like structure, which is great for forests
	// blockingRatio: float between 0f and 1f; 1f is no blocking and 0f is all blocking
	// blockingSize: how large thick the walls of the maze are, recommended .2f 
	public void PerlinGenerator(List<Tile> tiles, TilePlacer spritePlacer, float blockingRatio, float blockingSize) {

		foreach (Tile tile in tiles) {
			float noise = Mathf.PerlinNoise((float)tile.x * blockingSize, (float)tile.y * blockingSize);
			if (noise > blockingRatio && !tile.path && tile.item == null) {
				tile.blocking = true;
				spritePlacer(tile.x, tile.y);
			}
		}

	}

	public GameObject getGroundTile() {
		return this.groundTiles[Random.Range(0, this.groundTiles.Length)];
	}
	
	public GameObject getBlockingTile() {
		return this.blockingTiles[Random.Range(0, this.blockingTiles.Length)];
	}

	public void placeBlockingTile(int x, int y) {
		this.GetComponent<RoomManager>().PlaceItem(this.getBlockingTile(), x, y);
	}

	public GameObject getEnemy() {
		return this.enemies[Random.Range(0, this.enemies.Length)];
	}

	public abstract void RandomBlocking(List<Tile> region);
	public abstract int getBiomeNumber();
}

