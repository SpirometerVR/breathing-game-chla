﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Additional class to create Ocean tiles.
public class Tile
{
    public GameObject theTile;
    public float creationTime;

    public Tile(GameObject gbj, float ct)
    {
        theTile = gbj;
        creationTime = ct;
    }
}

public class GenerateMap : MonoBehaviour
{
    // Array with different ocean tiles.
    public GameObject[] plane;
    public GameObject player;

    // Ocean prefab size.
    private int planeSize = 100;

    // Number of additional tiles on the Z and X axes.
    private int halfTilesZ = 5;
    private int halfTilesX = 5;

    Vector3 startPos;

    // Store ocean tiles by position.
    Hashtable tiles = new Hashtable();

    // Start is called before the first frame update
    void Start()
    {
        // Initial position at (0, 0, 0)
        this.gameObject.transform.position = Vector3.zero;
        startPos = Vector3.zero;

        float updateTime = Time.realtimeSinceStartup;

        // Generate tiles on either side of current tile on Z axis.
        for (int x = -halfTilesX; x <= halfTilesX; x++)
        {
            for (int z = -halfTilesZ; z <= halfTilesZ; z++)
            {
                Vector3 pos = new Vector3((x * (planeSize) + startPos.x), -30, (z * (planeSize) + startPos.z));
                GameObject gbj = (GameObject)Instantiate(plane[RandomOceanGenerator()], pos, Quaternion.identity);

                // Create Ocean tiles to be stored in HashTable.
                string tileName = "Ocean_" + ((int)(pos.x)).ToString() + "_" + ((int)(pos.z)).ToString();
                gbj.name = tileName;
                Tile tile = new Tile(gbj, updateTime);
                tiles.Add(tileName, tile);
            }
        }
        
    }

	// Update is called once per frame
	void Update()
	{
		// Determine how far player moved from last Ocean tile update.
		int xMove = (int)(player.transform.position.x - startPos.x);
		int zMove = (int)(player.transform.position.z - startPos.z);

		// If the player's position is close to edge of an Ocean tile, generate a new tile.
		if ((Mathf.Abs(xMove) >= planeSize || (Mathf.Abs(zMove) >= planeSize)))
		{
			float updateTime = Time.realtimeSinceStartup;

			// Round down on player position.
			int playerX = (int)(Mathf.Floor((int)player.transform.position.x / (int)planeSize) * planeSize);
			int playerZ = (int)(Mathf.Floor(player.transform.position.z / planeSize) * planeSize);

			// Generate tiles on either size of current tile on Z axis.
			for (int x = -halfTilesX; x <= halfTilesX; x++)
			{
				for (int z = -halfTilesZ + 1; z <= halfTilesZ + 1; z++)
				{
					Vector3 pos = new Vector3((x * (planeSize) + playerX), -30, (z * (planeSize) + playerZ));

					string tileName = "Ocean_" + ((int)(pos.x)).ToString() + "_" + ((int)(pos.z)).ToString();

					// Add new tile if it doesn't exist in hashtable
					if (!tiles.ContainsKey(tileName))
					{
						GameObject gbj = (GameObject)Instantiate(plane[RandomOceanGenerator()], pos, Quaternion.identity);
						gbj.name = tileName;
						Tile tile = new Tile(gbj, updateTime);
						tiles.Add(tileName, tile);
					}
					// Or update time if the tile already exists.
					else
					{
						(tiles[tileName] as Tile).creationTime = updateTime;
					}
				}
			}

			// Destroy all tiles that weren't just created or have had their time updated
			// and put new tiles and kept tiles in hashtable
			Hashtable newOcean = new Hashtable();
			foreach (Tile ocn in tiles.Values)
			{
				if (ocn.creationTime != updateTime)
				{
					Destroy(ocn.theTile);
				}
				else
				{
					// Keep ocean tile.
					newOcean.Add(ocn.theTile.name, ocn);
				}
			}
			// Copy new hashtable to old one.
			tiles = newOcean;
			// Update start position to current player position.
			startPos = player.transform.position;
		}
	}

	// Randomly choose an ocean tile from the array.
	private int RandomOceanGenerator()
    {
        //int[] values = {1, 2, 3, 4, 5};
        //int picker = Random.Range(0, values.Length);
        //return values[picker];
        return Random.Range(0, plane.Length);
    }


}
