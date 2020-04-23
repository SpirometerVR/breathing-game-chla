using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    // Regular offset for camera.
    private static Vector3 offset = new Vector3(0f, 38f, -54f);
    // Offset when player is exhaling to simulate acceleration.
    private Vector3 zoomOffset = Vector3.Scale(offset, new Vector3(0f, 1.2f, 1.7f));

    private float speed = 10f;
    private RocketController playerScript;

    Camera mainCamera;
    private int zoom = 75;
    private int normal = 60;
    private float smooth = 5f;
    private bool isZoomed = false;

    void Start()
    {
        mainCamera = Camera.main;
        playerScript = player.GetComponent<RocketController>();
    }

    // Update is called once per frame
    void Update()
    {
        // Keep camera at a position behind the player.
        transform.position = player.transform.position + offset;
        isZoomed = (playerScript.exhalePhase && playerScript.exhaleIsOn) ? true : false;

        // Push camera back on exhale
        //if (isZoomed) 
        //{
        //    mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, zoom, Time.deltaTime*smooth);
        //} else {
        //    mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, normal, Time.deltaTime*smooth);
        //}
    }
}
