using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireController : MonoBehaviour
{
    public GameObject player;
    private ParticleSystem flames;
    private RocketController playerScript;
    public Vector3 offset = new Vector3(0f, -0.6f, -16.1f);

    // Start is called before the first frame update
    void Start()
    {
        // Find the flame GameObjects.
        playerScript = player.GetComponent<RocketController>();
        flames = GetComponent<ParticleSystem>();

        // Manually stop the flames to be safe.
        flames.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        // Keep the flames at the engine positions.
        //transform.position = player.transform.position + offset;
        //transform.rotation = Quaternion.Euler(player.transform.rotation.eulerAngles.x + 180, player.transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);

        // Only play flames when player is exhaling.
        if (playerScript.exhaleIsOn && playerScript.exhalePhase)
        {
            flames.Play();
        }
        else
        {
            flames.Stop();
        }
    }
}
