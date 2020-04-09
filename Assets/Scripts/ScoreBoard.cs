using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoard : MonoBehaviour
{
    public float coinScore;
    public float treasureScore;
    public float totalCoins;
    public float totalTreasure;

    private MainBoatController player;

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
        inhaleScore = GameObject.FindGameObjectWithTag("Treasure Score").GetComponent<Text>();
        exhaleScore = GameObject.FindGameObjectWithTag("Coin Score").GetComponent<Text>();
        finalScore = GameObject.FindGameObjectWithTag("Final Score").GetComponent<Text>();
        spedometerText = GameObject.FindGameObjectWithTag("Spedometer").GetComponent<Text>();
        player = GameObject.FindGameObjectWithTag("Boat").GetComponent<MainBoatController>();

        inhaleScoreCard = GameObject.FindGameObjectWithTag("Treasure Score").GetComponent<ScoreBoard>();
        exhaleScoreCard = GameObject.FindGameObjectWithTag("Coin Score").GetComponent<ScoreBoard>();
        finalScoreCard = GameObject.FindGameObjectWithTag("Final Score").GetComponent<ScoreBoard>();
        spedometerCard = GameObject.FindGameObjectWithTag("Spedometer").GetComponent<ScoreBoard>();

        totalCoins = player.exhaleTargetTime * player.cycles;
        totalTreasure = player.cycles;
    }

    // Update is called once per frame
    void Update()
    {
        if (player.gameOver)
        {
            //finalScore.gameObject.SetActive(true);
            finalScore.text = "Final Score: " + (treasureScore + coinScore) + "/" + (totalCoins + totalTreasure);
            inhaleScore.text = "";
            exhaleScore.text = "";
            spedometerText.text = "";
        }
        else
        {
            finalScore.text = "";
            spedometerText.text = "Speed: " + player.speed + " mph";
            inhaleScore.text = "Treasure: " + treasureScore;
            exhaleScore.text = "Coins: " + coinScore;
        }
    }
}
