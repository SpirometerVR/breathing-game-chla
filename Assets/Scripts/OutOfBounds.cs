using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OutOfBounds : MonoBehaviour
{
    public GameObject player;
    public RocketController playerScript;
    private GameObject OutOfBoundsCanvas;
    private Text outOfBoundsText;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Rocket");
        playerScript = player.GetComponent<RocketController>();
        outOfBoundsText = GameObject.FindGameObjectWithTag("Out Of Bounds").GetComponent<Text>();
        OutOfBoundsCanvas = GameObject.FindGameObjectWithTag("Out Of Bounds Canvas");
    }

    // Update is called once per frame
    void Update()
    {
        // Only print error when the camera view is out of bounds.
        if(playerScript.inBounds)
		{
            // Lock Rotation on X and Z Axis.
            OutOfBoundsCanvas.transform.rotation = Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y, 0);
            outOfBoundsText.text = "";
		}
        else
		{
            // Lock Rotation on X and Z Axis.
            OutOfBoundsCanvas.transform.rotation = Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y, 0);
            outOfBoundsText.text = "RETURN TO SHIP";
		}
    }
}
