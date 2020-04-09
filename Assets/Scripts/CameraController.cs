using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    private static Vector3 offset = new Vector3(0f, 22.2f, -25.68f);
    private Vector3 zoomOffset = Vector3.Scale(offset, new Vector3(0f, 1.2f, 1.7f));

    private float speed = 10f;
    private MainBoatController playerScript;

    void Start()
    {
        playerScript = player.GetComponent<MainBoatController>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = player.transform.position + offset;

        if (playerScript.exhalePhase && playerScript.exhaleIsOn)
        {
            float exhaleDuration = (float)System.Math.Round(playerScript.exhaleDuration, 1);

            if (exhaleDuration < 1)
            {
                transform.position = Vector3.Lerp(player.transform.position + offset, player.transform.position + zoomOffset, exhaleDuration);
            } else {
                transform.position = Vector3.Lerp(player.transform.position + offset, player.transform.position + zoomOffset, 1);
            }
        }

        if (playerScript.inhalePhase && !playerScript.inhaleIsOn)
        {
            float breakDuration = (float)System.Math.Round(playerScript.breakDuration, 1);

            if (breakDuration < 1)
            {
                transform.position = Vector3.Lerp(player.transform.position + zoomOffset, player.transform.position + offset, breakDuration);
            } else {
                transform.position = Vector3.Lerp(player.transform.position + zoomOffset, player.transform.position + offset, 1);
            }
        }
    }
}
