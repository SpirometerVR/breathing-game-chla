using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Additional class to create Space tiles.
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
    // Array with different planets + asteroids for outside view.
    public GameObject[] outsidePlane;
    // Array with different asteroids for inside view.
    public GameObject[] insidePlane;
    public GameObject player;

    // Space prefab size.
    private int planeSize = 500;

    // Number of additional tiles on the Z and X axes.
    private int halfTilesZ = 2;
    private int halfTilesX = 2;

    Vector3 startPos;

    // Store space tiles by position.
    Hashtable tiles = new Hashtable();

    // Start is called before the first frame update
    void Start()
    {
        // Initial position at (0, 0, 0)
        this.gameObject.transform.position = Vector3.zero;
        startPos = Vector3.zero;

        float updateTime = Time.realtimeSinceStartup;

        // Generate tiles on either side of current tile on Z and X axes.
        for (int x = -halfTilesX; x <= halfTilesX; x++)
        {
            for (int z = -halfTilesZ; z <= halfTilesZ; z++)
            {
                Vector3 pos = new Vector3((x * (planeSize) + startPos.x), 0, (z * (planeSize) + startPos.z));
                GameObject gbj;
                // If it is a tile on the left side, only choose from planes with planets.
                if (x == -halfTilesX)
                {
                    gbj = (GameObject)Instantiate(outsidePlane[RandomPlaneGeneratorOutside()], pos, Quaternion.identity);
                }
                // If it is a tile on the right side, only choose from planes with planets, but rotate plane 180.
                else if (x == halfTilesX)
                {
                    GameObject planet = outsidePlane[RandomPlaneGeneratorOutside()];
                    gbj = (GameObject)Instantiate(planet, pos, Quaternion.Euler(planet.transform.rotation.eulerAngles.x, planet.transform.rotation.eulerAngles.y + 180, planet.transform.rotation.eulerAngles.z));
                }
                // Otherwise choose inside planes with only asteroids.
                else
                {
                    gbj = (GameObject)Instantiate(insidePlane[RandomPlaneGeneratorInside()], pos, Quaternion.identity);
                }
                // Create Space tiles to be stored in HashTable.
                string tileName = "Space_" + ((int)(pos.x)).ToString() + "_" + ((int)(pos.z)).ToString();
                gbj.name = tileName;
                Tile tile = new Tile(gbj, updateTime);
                tiles.Add(tileName, tile);
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        // Determine how far player moved from last Space tile update.
        int xMove = (int)(player.transform.position.x - startPos.x);
        int zMove = (int)(player.transform.position.z - startPos.z);

        // If the player's position is close to edge of an Space tile, generate a new tile.
        if ((Mathf.Abs(xMove) >= planeSize || (Mathf.Abs(zMove) >= planeSize)))
        {
            float updateTime = Time.realtimeSinceStartup;

            // Round down on player position.
            int playerX = (int)(Mathf.Floor((int)player.transform.position.x / (int)planeSize) * planeSize);
            int playerZ = (int)(Mathf.Floor(player.transform.position.z / planeSize) * planeSize);

            // Generate tiles on either size of current tile on Z axis.
            for (int x = -halfTilesX; x <= halfTilesX; x++)
            {
                for (int z = -halfTilesZ; z <= halfTilesZ; z++)
                {
                    Vector3 pos = new Vector3((x * (planeSize) + playerX), 0, (z * (planeSize) + playerZ));

                    string tileName = "Space_" + ((int)(pos.x)).ToString() + "_" + ((int)(pos.z)).ToString();

                    // Add new tile if it doesn't exist in hashtable
                    if (!tiles.ContainsKey(tileName))
                    {
                        GameObject gbj;
                        // If it is a tile on the left side, only choose from planes with planets.
                        if (x == -halfTilesX)
                        {
                            gbj = (GameObject)Instantiate(outsidePlane[RandomPlaneGeneratorOutside()], pos, Quaternion.identity);
                        }
                        // If it is a tile on the right side, only choose from planets with planets, but rotate plane 180.
                        else if (x == halfTilesX)
                        {
                            GameObject planet = outsidePlane[RandomPlaneGeneratorOutside()];
                            gbj = (GameObject)Instantiate(planet, pos, Quaternion.Euler(planet.transform.rotation.eulerAngles.x, planet.transform.rotation.eulerAngles.y + 180, planet.transform.rotation.eulerAngles.z));
                        }
                        // Otherwise choose inside planes with only asteroids.
                        else
                        {
                            gbj = (GameObject)Instantiate(insidePlane[RandomPlaneGeneratorInside()], pos, Quaternion.identity);
                        }
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
            Hashtable newSpace = new Hashtable();
            foreach (Tile spc in tiles.Values)
            {
                if (spc.creationTime != updateTime)
                {
                    Destroy(spc.theTile);
                }
                else
                {
                    // Keep space tile.
                    newSpace.Add(spc.theTile.name, spc);
                }
            }
            // Copy new hashtable to old one.
            tiles = newSpace;
            // Update start position to current player position.
            startPos = player.transform.position;
        }
    }

    // Randomly choose an plane tile from the array.
    private int RandomPlaneGeneratorOutside()
    {
        return Random.Range(0, outsidePlane.Length);
    }

    // Randomly choose an plane tile from the array.
    private int RandomPlaneGeneratorInside()
    {
        return Random.Range(0, insidePlane.Length);
    }


}
