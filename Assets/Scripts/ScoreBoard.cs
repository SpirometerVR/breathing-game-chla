using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoard : MonoBehaviour
{
    public float diamondScore;
    public float totalDiamonds;
    public float totalTreasure;

    private RocketController player;

    private Text inhaleScore;
    private Text exhaleScore;
    private Text finalScore;
    private Text spedometerText;

    private ScoreBoard exhaleScoreCard;
    private ScoreBoard inhaleScoreCard;
    private ScoreBoard finalScoreCard;
    private ScoreBoard spedometerCard;

    // Start is called before the first frame update
    void Start()
    {
        // Find all of the score boards under the Canvas object.
        exhaleScore = GameObject.FindGameObjectWithTag("Diamond Score").GetComponent<Text>();
        finalScore = GameObject.FindGameObjectWithTag("Final Score").GetComponent<Text>();
        spedometerText = GameObject.FindGameObjectWithTag("Spedometer").GetComponent<Text>();

        // Find the player.
        player = GameObject.FindGameObjectWithTag("Rocket").GetComponent<RocketController>();

        // Initialize scoreboard objects.
        exhaleScoreCard = GameObject.FindGameObjectWithTag("Diamond Score").GetComponent<ScoreBoard>();
        finalScoreCard = GameObject.FindGameObjectWithTag("Final Score").GetComponent<ScoreBoard>();
        spedometerCard = GameObject.FindGameObjectWithTag("Spedometer").GetComponent<ScoreBoard>();

        // Set the target score based on the exhale cycles.
        totalDiamonds = player.exhaleTargetTime * player.cycles;
    }

    // Update is called once per frame
    void Update()
    {
        // If the game is over, print the final score alone.
        if (player.gameOver)
        {
            finalScore.text = "Final Score: " + (diamondScore) + "/" + (totalDiamonds);
            exhaleScore.text = "";
            spedometerText.text = "";
        }
        // Otherwise, print the current score.
        else
        {
            finalScore.text = "";
            spedometerText.text = "Speed: " + player.speed + " mph";
            exhaleScore.text = "Diamonds: " + diamondScore;
        }
    }
}
