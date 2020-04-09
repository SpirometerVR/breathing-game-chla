using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudController : MonoBehaviour
{
    private float speed = 12.5f;
    private Rigidbody rigidBody;
    private GameObject player;
    private MainBoatController playerScript;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Boat");
        playerScript = player.GetComponent<MainBoatController>();
    }

    // Update is called once per frame
    void Update()
    {
        // Move the treasure object towards the player when inhaling
        if (playerScript.inhalePhase && playerScript.inhaleIsOn && playerScript.inhaleDuration > 0.4f)
        {
			Vector3 playerDirection = (player.transform.position - transform.position).normalized;
			Vector3 lookDirection = new Vector3(playerDirection.x, 0, -1);
            transform.Translate(lookDirection * speed * Time.deltaTime);
            // If the object is past the boat, destroy it. 
            if (transform.position.z < player.transform.position.z)
            {
                Destroy(gameObject);
            }
        }
    }
}
