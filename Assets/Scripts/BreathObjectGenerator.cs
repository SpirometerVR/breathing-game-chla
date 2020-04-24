using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreathObjectGenerator : MonoBehaviour
{
    public GameObject diamondOne;
    public GameObject remainingDiamonds;
    public GameObject fuel;

    private GameObject player;
    private RocketController playerScript;

    private int diamondCount = 1;
    private bool firstDiamondSpawn = false;
    public bool inhaleSpawned = false;
    private bool exhaleSpawned = false;
    private float initialDiamondDistance = 130f;
    private float remainingDiamondDistance = 0f;

    private bool isCoroutineExecutingFuel = false;
    private bool isCoroutineExecutingDiamond = false;
    private bool isCoroutineExecutingDiamondDestroy = false;
    private bool isCoroutineExecutingFuelDestroy = false;

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
                // Destroy any existing diamonds for the inhale phase.
                StartCoroutine(DestroyDiamonds());
                // If the fuels have not been spawned yet, spawn them.
                if (!inhaleSpawned)
                {
                    StartCoroutine(SpawnFuelItems());
                }
            }
            if (playerScript.exhalePhase)
            {
                //Destroy all fuel objects still in the game during exhale phase.
                StartCoroutine(DestroyFuel());
                // If the diamonds have not been spawned yet, spawn them.
                if (!exhaleSpawned)
                {
                    // Spawn the first diamond first to determine the position of the other diamonds.
                    if (!firstDiamondSpawn)
                    {
                        StartCoroutine(SpawnDiamondItems());
                    }
                    // Spawn the remaining diamonds based on the exhale duration.
                    else
                    {
                        SpawnRemainingDiamonds();
                        // Reset the diamond flags.
                        if (diamondCount == playerScript.exhaleTargetTime)
                        {
                            diamondCount = 1;
                            firstDiamondSpawn = false;
                            remainingDiamondDistance = 0f;
                            exhaleSpawned = true;
                        }
                    }
                }
            }
        }
    }

    // Spawn the first diamond in front of the Rocket.
    private void SpawnFirstDiamond()
    {
        Vector3 playerPosition = transform.position;
        // Need cross product to produce diamonds in front of Rocket.
        Vector3 playerForward = Vector3.Cross(transform.forward, new Vector3(0, 1, 0));
        // Determine the right rotation for the diamond gameObject.
        Quaternion playerRotation = Quaternion.Euler(90, 180, 0);
        // Determine the spawn position of the first diamond based on the Rocket's position.
        Vector3 spawnPosition = new Vector3(transform.position.x + RandomXPosition(), 0, playerPosition.z) + new Vector3(0,0,1) * initialDiamondDistance;
        Instantiate(diamondOne, spawnPosition, playerRotation);
        firstDiamondSpawn = true;
        inhaleSpawned = false;
    }

    // Spawn the remaining number of diamonds one after another. The number is based on the exhaleDuration float from the RocketController.
    private void SpawnRemainingDiamonds()
    {
        // Need cross product to produce diamonds in front of Rocket.
        Vector3 playerForward = Vector3.Cross(transform.forward, new Vector3(0, 1, 0));
        Quaternion playerRotation = Quaternion.Euler(90, 180, 0);
        // Continue spawning diamonds until their target quantity is reached.
        if (diamondCount < playerScript.exhaleTargetTime)
        {
            remainingDiamondDistance += 320;
            // Spawn the diamond behind the most recent diamond spawned.
            Vector3 spawnPosition = GameObject.FindGameObjectWithTag("Diamond").transform.position + new Vector3(RandomXPosition() / remainingDiamondDistance, 0, 1) * remainingDiamondDistance;
            Instantiate(remainingDiamonds, spawnPosition, playerRotation);
            diamondCount++;
        }
    }

    private void SpawnFuel()
    {
        Vector3 playerPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        Quaternion playerRotation = Quaternion.Euler(0, 0, 0);
        // Spawn fuel relative to the rocket position.
		Vector3 spawnPosition = playerPosition + new Vector3(0, 0, -30);
		Instantiate(fuel, spawnPosition, playerRotation);
        // Set flags so that inhale spawn is true and update cycle counter.
        inhaleSpawned = true;
        exhaleSpawned = false;
        playerScript.cycleCounter += 1;
    }

    private float RandomXPosition()
    {
        return Random.Range(-50, 50);
    }

    private IEnumerator SpawnDiamondItems()
    {
        if (isCoroutineExecutingDiamond)
        {
            yield break;
        }
        isCoroutineExecutingDiamond = true;
        // Wait 2 seconds to spawn the first diamond
        yield return new WaitForSeconds(2f);
        SpawnFirstDiamond();
        isCoroutineExecutingDiamond = false;
    }


    private IEnumerator DestroyDiamonds()
    {
        if(isCoroutineExecutingDiamondDestroy)
        {
            yield break;
        }
        isCoroutineExecutingDiamondDestroy = true;
        // Wait 1.5 seconds before destroying diamonds if the exhale is off
        yield return new WaitForSeconds(1f);
        // Destroy all diamond objects.
        Destroy(GameObject.FindGameObjectWithTag("Diamond"));
        Destroy(GameObject.FindGameObjectWithTag("Diamond Two"));
        isCoroutineExecutingDiamondDestroy = false;
    }

    private IEnumerator SpawnFuelItems()
    {
        if (isCoroutineExecutingFuel)
        {
            yield break;
        }
        isCoroutineExecutingFuel = true;
        // Wait 3.5 seconds to spawn the new fuels
        yield return new WaitForSeconds(3.5f);
        SpawnFuel();
        isCoroutineExecutingFuel = false;
    }

    private IEnumerator DestroyFuel()
    {
        if (isCoroutineExecutingFuelDestroy)
        {
            yield break;
        }
        isCoroutineExecutingFuelDestroy = true;
		// Wait 0.8 seconds to destory the remaining fuels
		yield return new WaitForSeconds(0.8f);
        // Destroy all fuel objects.
        Destroy(GameObject.FindGameObjectWithTag("Fuel"));
        Destroy(GameObject.FindGameObjectWithTag("Right Fuel"));
        Destroy(GameObject.FindGameObjectWithTag("Left Fuel"));
        Destroy(GameObject.FindGameObjectWithTag("Middle Fuel"));
        isCoroutineExecutingFuelDestroy = false;
    }
}
