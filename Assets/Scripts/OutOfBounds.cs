using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfBounds : MonoBehaviour
{
    public GameObject player;
    public RocketController playerScript;
    private GameObject canvasObject;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Rocket");
        playerScript = player.GetComponent<RocketController>();
        canvasObject = GameObject.FindGameObjectWithTag("Out Of Bounds");
        canvasObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(playerScript.outOfBounds)
		{
            canvasObject.SetActive(true);
		}
        else
		{
            canvasObject.SetActive(false);
		}
    }
}
