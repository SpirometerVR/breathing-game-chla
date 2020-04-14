using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelController : MonoBehaviour
{
    private float speed = (12.5f/2f);
    private Rigidbody rigidBody;
    private GameObject player;
    private RocketController playerScript;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Rocket");
        playerScript = player.GetComponent<RocketController>();
    }

    // Update is called once per frame
    void Update()
    {
        // Move the treasure object towards the player when inhaling
        if (playerScript.inhalePhase && playerScript.inhaleIsOn)
        {
			Vector3 playerDirection = (player.transform.position - transform.position).normalized;
			Vector3 lookDirection = new Vector3(playerDirection.x, 0, playerDirection.z);
			transform.Translate(playerDirection * speed * Time.deltaTime);
			// If the object is past the boat, destroy it. 
			if (transform.position.z >= player.transform.position.z)
			{
				Destroy(gameObject);
			}
			//alphaLevel -= (1 / playerScript.inhaleTargetTime);
			//cloud.color = new Color(1, 1, 1, alphaLevel);
		}
    }
}
