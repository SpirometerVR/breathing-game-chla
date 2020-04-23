using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextMover : MonoBehaviour
{
    public GameObject player;
    private Vector3 offset = new Vector3(0, 0, 117.7f);

    // Start is called before the first frame update
    void Start() {}

    // Update is called once per frame
    void Update()
    {
        // Move the main scoreboard object so that it moves with the rocket position.
        transform.position = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z) + offset;

    }
}
