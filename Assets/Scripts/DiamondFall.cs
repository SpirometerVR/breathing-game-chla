using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiamondFall : MonoBehaviour
{
    private GameObject player;
    private GameObject scoreBoard;
    private RocketController playerScript;
    private float speed = 25f;
    private Vector3 offset = new Vector3(0, -3, 0);
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
        if (playerScript.exhaleIsOn && playerScript.exhalePhase)
        {
            transform.position = new Vector3(scoreBoard.transform.position.x, scoreBoard.transform.position.y, scoreBoard.transform.position.z) + offset;
        }
        else
        { 
            // Adding translation in the Z due to rotation. This is equivalent to negative Y axis movement.
            transform.Translate(new Vector3(0, 0, 1) * speed * Time.deltaTime);
            if (transform.position.y < player.transform.position.y)
            {
                Destroy(gameObject);
                diamondScores.diamondScore -= 1;
            }
        }
    }
}
