using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreathObjectGenerator : MonoBehaviour
{
    public GameObject coinOne;
    public GameObject remainingCoins;
    public GameObject cloud;

    private GameObject player;
    private RocketController playerScript;

    private int coinCount = 1;
    private bool firstCoinSpawn = false;
    private bool inhaleSpawned = false;
    private bool exhaleSpawned = false;
    private float initialCoinDistance = 130f;
    private float remainingCoinDistance = 0f;

    private bool isCoroutineExecutingCloud = false;
    private bool isCoroutineExecutingCoin = false;
    private bool isCoroutineExecutingCoinDestroy = false;
    private bool isCoroutineExecutingCloudDestroy = false;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Rocket");
        playerScript = player.GetComponent<RocketController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerScript.gameOver)
        {
            if (playerScript.inhalePhase)
            {
                if(playerScript.breakIsOn && playerScript.inhaleDuration > 0.5 && !playerScript.inhaleSuccess)
				{
                    StartCoroutine(DestroyCloud());
					StartCoroutine(SpawnCloudItems());
				}
                // Destroy any existing coins for the inhale phase.
                StartCoroutine(DestroyCoins());
                // If the clouds have not been spawned yet, spawn them.
                if (!inhaleSpawned)
                {
                    StartCoroutine(SpawnCloudItems());
                }
            }
            if (playerScript.exhalePhase)
            {
                //Destroy all cloud objects still in the game during exhale phase.
                StartCoroutine(DestroyCloud());
                // If the coins have not been spawned yet, spawn them.
                if (!exhaleSpawned)
                {
                    // Spawn the first coin first to determine the position of the other coins.
                    if (!firstCoinSpawn)
                    {
                        StartCoroutine(SpawnCoinItems());
                    }
                    // Spawn the remaining coins based on the exhale duration.
                    else
                    {
                        SpawnRemainingCoins();
                        // Reset the coin flags.
                        if (coinCount == playerScript.exhaleTargetTime)
                        {
                            coinCount = 1;
                            firstCoinSpawn = false;
                            remainingCoinDistance = 0f;
                            exhaleSpawned = true;
                        }
                    }
                }
            }
        }
    }

    // Spawn the first coin in front of the Rocket.
    private void SpawnFirstCoin()
    {
        Vector3 playerPosition = transform.position;
        // Need cross product to produce coins in front of Rocket.
        Vector3 playerForward = Vector3.Cross(transform.forward, new Vector3(0, 1, 0));
        // Determine the right rotation for the coin gameObject.
        Quaternion playerRotation = Quaternion.Euler(90, 180, 0);
        // Determine the spawn position of the first coin based on the Rocket's position.
        Vector3 spawnPosition = new Vector3(RandomXPosition(), 0, playerPosition.z) + new Vector3(0,0,1) * initialCoinDistance;
        Instantiate(coinOne, spawnPosition, playerRotation);
        firstCoinSpawn = true;
        inhaleSpawned = false;
    }

    // Spawn the remaining number of coins one after another. The number is based on the exhaleDuration float from the RocketController.
    private void SpawnRemainingCoins()
    {
        // Need cross product to produce coins in front of Rocket.
        Vector3 playerForward = Vector3.Cross(transform.forward, new Vector3(0, 1, 0));
        Quaternion playerRotation = Quaternion.Euler(90, 180, 0);
        // Continue spawning coins until their target quantity is reached.
        if (coinCount < playerScript.exhaleTargetTime)
        {
            remainingCoinDistance += 320;
            // Spawn the coin behind the most recent coin spawned.
            Vector3 spawnPosition = GameObject.FindGameObjectWithTag("Diamond").transform.position + new Vector3(RandomXPosition() / remainingCoinDistance, 0, 1) * remainingCoinDistance;
            Instantiate(remainingCoins, spawnPosition, playerRotation);
            coinCount++;
        }
    }

    private void SpawnCloud()
    {
        Vector3 playerPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        Quaternion playerRotation = Quaternion.Euler(0, 0, 0);
		Vector3 spawnPosition = playerPosition + new Vector3(0, 0, -30);
		Instantiate(cloud, spawnPosition, playerRotation);
        inhaleSpawned = true;
        exhaleSpawned = false;
        // Reset the inhale Duration to 0 every inhale cycle.
        playerScript.inhaleDuration = 0;
        playerScript.cycleCounter += 1;
    }

    private float RandomXPosition()
    {
        return Random.Range(-50, 50);
    }

    private IEnumerator SpawnCoinItems()
    {
        if (isCoroutineExecutingCoin)
        {
            yield break;
        }
        isCoroutineExecutingCoin = true;
        // Wait 1.5 seconds to spawn the first coin
        yield return new WaitForSeconds(1.5f);
        SpawnFirstCoin();
        isCoroutineExecutingCoin = false;
    }


    private IEnumerator DestroyCoins()
    {
        if(isCoroutineExecutingCoinDestroy)
        {
            yield break;
        }
        isCoroutineExecutingCoinDestroy = true;
        // Wait 0.8 seconds before destroying coins if the exhale is off
        yield return new WaitForSeconds(2f);
        Destroy(GameObject.FindGameObjectWithTag("Diamond"));
        Destroy(GameObject.FindGameObjectWithTag("Diamond Two"));
        isCoroutineExecutingCoinDestroy = false;
    }

    private IEnumerator SpawnCloudItems()
    {
        if (isCoroutineExecutingCloud)
        {
            yield break;
        }
        isCoroutineExecutingCloud = true;
        // Wait 3.5 seconds to spawn the new clouds
        yield return new WaitForSeconds(3.5f);
        SpawnCloud();
        isCoroutineExecutingCloud = false;
    }

    private IEnumerator DestroyCloud()
    {
        if (isCoroutineExecutingCloudDestroy)
        {
            yield break;
        }
        isCoroutineExecutingCloudDestroy = true;
        // Reset inhaleDuration to zero so that clouds are not continuosly destroyed.
        playerScript.inhaleDuration = 0;
		// Wait 0.8 seconds to destory the remaining clouds
		yield return new WaitForSeconds(0.8f);
		GameObject[] fuels = GameObject.FindGameObjectsWithTag("Fuel");
        foreach (GameObject fuel in fuels)
		{
            GameObject.Destroy(fuel);
        }
        isCoroutineExecutingCloudDestroy = false;
    }
}
