using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    private static Vector3 offset = new Vector3(0f, 25.23f, -40f);
    private Vector3 zoomOffset = Vector3.Scale(offset, new Vector3(0f, 1.2f, 1.7f));

    private float speed = 10f;
    private MainBoatController playerScript;

    Camera mainCamera;
    private int zoom = 75;
    private int normal = 60;
    private float smooth = 5f;
    private bool isZoomed = false;

    void Start()
    {
        mainCamera = Camera.main;
        playerScript = player.GetComponent<MainBoatController>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = player.transform.position + offset;

        isZoomed = (playerScript.exhalePhase && playerScript.exhaleIsOn) ? true : false;

        if (isZoomed) 
        {
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, zoom, Time.deltaTime*smooth);
        } else {
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, normal, Time.deltaTime*smooth);
        }
    }
}
