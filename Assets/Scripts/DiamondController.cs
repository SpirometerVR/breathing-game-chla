using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiamondController : MonoBehaviour
{
    private GameObject scoreBoard;
    private GameObject player;
    private RocketController playerScript;
    private float speed = 200f;
    private Vector3 offset = new Vector3(-10, 10, 0);
    private ScoreBoard diamondScores;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Rocket");
        playerScript = player.GetComponent<RocketController>();
        scoreBoard = GameObject.FindGameObjectWithTag("Coin Score");
        diamondScores = GameObject.FindGameObjectWithTag("Coin Score").GetComponent<ScoreBoard>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerScript.exhalePhase && playerScript.exhaleIsOn)
        {
            transform.position = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z) + offset;
        }
        else
        {
            // Move the diamonds towards the scoreboard.
            transform.position = Vector3.MoveTowards(transform.position, scoreBoard.transform.position, speed * Time.deltaTime);
            if (transform.position == scoreBoard.transform.position)
            {
                Destroy(gameObject);
                diamondScores.diamondScore += 1;
            }
        }
    }
}
