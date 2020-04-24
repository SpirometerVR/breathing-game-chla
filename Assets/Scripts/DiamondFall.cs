using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiamondFall : MonoBehaviour
{
    private GameObject player;
    private GameObject scoreBoard;
    private RocketController playerScript;
    private float speed = 25f;
    private float timer = 1.5f;
    private Vector3 offset = new Vector3(0, -3, 0);
    private ScoreBoard diamondScores;

    // Start is called before the first frame update
    void Start()
    {
        // Find the appropriate Game Objects.
        player = GameObject.FindGameObjectWithTag("Rocket");
        playerScript = player.GetComponent<RocketController>();
        scoreBoard = GameObject.FindGameObjectWithTag("Diamond Score");
        diamondScores = GameObject.FindGameObjectWithTag("Diamond Score").GetComponent<ScoreBoard>();
    }

    // Update is called once per frame
    void Update()
    {
        // Keep the diamonds close to the scoreboard when player is exhaling.
        if (playerScript.exhaleIsOn && playerScript.exhalePhase)
        {
            transform.position = new Vector3(scoreBoard.transform.position.x, scoreBoard.transform.position.y - 3, scoreBoard.transform.position.z) + offset;
        }
        // Move the diamonds away from the scoreboard, destroy them, and reduce score by one.
        else
        {
            // Only drop diamond after timer hits zero.
            if (timer <= 0)
            {
                // Adding translation in the Z due to rotation. This is equivalent to negative Y axis movement.
                transform.Translate(new Vector3(0, 0, 1) * speed * Time.deltaTime);
                if (transform.position.y < player.transform.position.y)
                {
                    Destroy(gameObject);
                    diamondScores.diamondScore -= 1;
                    timer = 1.5f;
                }
            }
            // Otherwise maintain it at the position it currently is at and reduce timer.
            else
			{
                transform.position = new Vector3(scoreBoard.transform.position.x, scoreBoard.transform.position.y - 3, scoreBoard.transform.position.z) + offset;
                timer -= Time.deltaTime;
			}
        }
    }
}
