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
        // Find the appropriate GameObjects within the game.
        player = GameObject.FindGameObjectWithTag("Rocket");
        playerScript = player.GetComponent<RocketController>();
        scoreBoard = GameObject.FindGameObjectWithTag("Diamond Score");
        diamondScores = GameObject.FindGameObjectWithTag("Diamond Score").GetComponent<ScoreBoard>();
    }

    // Update is called once per frame
    void Update()
    {
        // If the player is exhaling, keep the diamond close to the ship.
        if (playerScript.exhalePhase && playerScript.exhaleIsOn)
        {
            transform.position = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z) + offset;
        }
        // Otherwise, move the diamonds to the scoreboard and update the score.
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
