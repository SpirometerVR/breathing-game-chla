using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainBoatController : MonoBehaviour
{
    public bool exhalePhase = false;
    public bool inhalePhase = true;
    public bool inhaleSuccess = false;

    // Public target times can be adjusted by doctor/patient.
    public float exhaleTargetTime = 1f;
    public float inhaleTargetTime = 1f;
    public float exhaleDuration;
    public float inhaleDuration;
	public float breakDuration;

	// Public cycles variable can be adjusted by doctor/patient.
	public float cycles = 5f;
    public float cycleCounter = 0f;
    public bool gameOver = false;
    public float speed;

    public AudioClip coin;
    public AudioClip crash;
    public AudioClip treasure;

    private float downTime = 0f;
    private float upTime = 0f;
	private float breakTime = 0f;
	private float exhaleStart = 0f;
    private float inhaleStart = 0f;
	private float breakStart = 0f;

	public bool exhaleIsOn = false;
    public bool inhaleIsOn = false;
    public bool breakIsOn = false;

    private float exhaleThresh = 1470f;
    private float inhaleTresh = 1200f;
    private float steadyThresh = 1340f;
    private float speedMultiplier = 0.5f;

    private AudioSource audio;
    private Renderer gameBoat;

    // Create GameObject to find OSC
    private GameObject OSC;
    // Hold OSC data in spirometer object
    private OSC spirometer;
    // Get the boat as a rigidbody
    private Rigidbody boatBody;

    private ScoreBoard treasureScores;
    private ScoreBoard coinScores;
    private ScoreBoard finalScores;
    private ScoreBoard spedometer;

    // Start is called before the first frame update
    void Start()
    {
        // Find the OSC Game Object.
        OSC = GameObject.Find("OSC");
        spirometer = OSC.GetComponent<OSC>();
        // Read input data from the M5 Stick on start.
        spirometer.SetAddressHandler("/Spirometer/C", ReceiveSpirometerData);

        // Get game renderer for the boat
        gameBoat = GetComponent<Renderer>();
        gameBoat.enabled = true;

        // Get rigid body and audio components for the boat.
        boatBody = GetComponent<Rigidbody>();
        audio = GetComponent<AudioSource>();

        // Find the score board objects for each respective scoreboard.
        treasureScores = GameObject.FindGameObjectWithTag("Treasure Score").GetComponent<ScoreBoard>();
        coinScores = GameObject.FindGameObjectWithTag("Coin Score").GetComponent<ScoreBoard>();
        finalScores = GameObject.FindGameObjectWithTag("Final Score").GetComponent<ScoreBoard>();
		spedometer = GameObject.FindGameObjectWithTag("Spedometer").GetComponent<ScoreBoard>();
		//spedometer = GameObject.FindGameObjectWithTag("Final Score").GetComponent<ScoreBoard>();

		// Manually set inhale phase to true at start of game.
		inhalePhase = true;
    }

    // Update is called once per frame.     
    void Update() {}

    // Place general movement in FixedUpdate to avoid shaking.
    private void FixedUpdate()
    {
        // Once the player has completed the number of cycles, set gameOver to true and destroy all existing gameObjects.
        if (cycleCounter > cycles)
        {
            gameOver = true;
            Destroy(GameObject.FindGameObjectWithTag("Cloud"));
        }
        // Otherwise, if the game is not over:
        if (!gameOver)
        {
			// Change boat direction based on camera in VR.
			transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, Camera.main.transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);

            // Take cross product to ensure that boat goes forward.
            Vector3 cameraVector = new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z);
            Vector3 forwardDir = Vector3.Cross(cameraVector, new Vector3(0, 1, 0));

            // Accelerate boat when player is exhaling or using upArrow input.
            if (exhalePhase && cameraBounds())
            {
                if (exhaleIsOn || Input.GetKey(KeyCode.UpArrow))
                {
                    if (Input.GetKey(KeyCode.UpArrow))
                    {
                        exhaleIsOn = true;
                        breakIsOn = false;
                    }
                    // reset inhaleDuration timer.
                    inhaleDuration = 0;
					breakDuration = 0;
					// Start timer to determine how long the breath is exhaled.
					downTime = Time.time;
                    // Add force to the boat to push it.
                    boatBody.AddRelativeForce(new Vector3(cameraVector.x, 0, cameraVector.z) * speedMultiplier, ForceMode.VelocityChange);
                    // Determine how long the exhale is or how long upArrow is being held down for.
                    exhaleDuration = downTime - exhaleStart;
					// Start counting the break time
					breakStart = Time.time;
                    // Reset inhaleSuccess flag
                    inhaleSuccess = false;
				}
				//TO ALLOW KEY BOARD PLAYABILITY, UNCOMMENT IF LOOP BELOW:
				if (!Input.GetKey(KeyCode.UpArrow))
				{
					exhaleIsOn = false;
				}
			}

            if (inhalePhase && cameraBounds())
            {
                // Pull air clouds towards the boat when inhaling or using Space key.
                if (inhaleIsOn ||  Input.GetKey(KeyCode.Space))
                {
                    if (Input.GetKey(KeyCode.Space))
                    {
                        inhaleIsOn = true;
                        breakIsOn = false;
                    }
                    // reset exhaleDuration & break duration timer.
                    exhaleDuration = 0;
					breakDuration = 0;
					// Start timer to determine how long the breath is inhaled.
					upTime = Time.time;
                    // Determine how long inhale was held for.
                    inhaleDuration = upTime - inhaleStart;
					// Start counting the break time
					breakStart = Time.time;
                    if (inhaleDuration >= inhaleTargetTime)
                    {
                        inhaleSuccess = true;
                    }
                }
				//TO ALLOW KEY BOARD PLAYABILITY, UNCOMMENT IF LOOP BELOW:
				if (!Input.GetKey(KeyCode.Space))
				{
					inhaleIsOn = false;
				}

			}

            // If the player is neither exhaling or inhaling:
            if (!exhaleIsOn && !inhaleIsOn)
            {
                // Repeat code for controller input
                if (!Input.GetKey(KeyCode.Space) && !Input.GetKey(KeyCode.UpArrow))
                {
                    inhaleIsOn = false;
                    exhaleIsOn = false;
                    breakIsOn = true;

                    // Snapshot this time. This time will be compared with the amount of time exhale/inhale is held to
                    // determine how long the inhale or exhale was.
                    exhaleStart = Time.time;
                    inhaleStart = Time.time;

					// Count how long the break is
					breakTime = Time.time;

					// Negate the force added to the boat via exhalation.
					var oppositeDirX = -boatBody.velocity;
                    boatBody.AddForce(oppositeDirX);

                    // Only count exhale and inhales that are longer than 1 second to remove erroneous air flow data.
                    // Once inhale or exhale is conducted and completed, switch cycles.
                    if (inhalePhase)
                    {
                        if (inhaleDuration >= inhaleTargetTime)
                        {
                            inhaleSuccess = true;
                            inhalePhase = false;
                            exhalePhase = true;
                        }
						else
						{
                            inhaleSuccess = false;
                            inhalePhase = true;
                            exhalePhase = false;
                        }
                    }
                    if (exhalePhase)
                    {
                        if (exhaleDuration >= 1)
                        {
                            inhalePhase = true;
                            exhalePhase = false;
                        }
                    }

					// Determine how long the break was
					breakDuration = breakTime - breakStart;
				}

                // Snapshot this time. This time will be compared with the amount of time exhale/inhale is held to
                // determine how long the inhale or exhale was.
                exhaleStart = Time.time;
                inhaleStart = Time.time;

				// Count how long the break is
				breakTime = Time.time;

				// Negate the force added to the boat via exhalation.
				var oppositeDir = -boatBody.velocity;
                boatBody.AddForce(oppositeDir);

                // Only count exhale and inhales that are longer than 1 second to remove erroneous air flow data.
                // Once inhale or exhale is conducted and completed, switch cycles.
                if (inhalePhase)
                {
                    if (inhaleDuration >= inhaleTargetTime)
                    {
                        inhaleSuccess = true;
                        inhalePhase = false;
                        exhalePhase = true;
                    }
                    else
                    {
                        inhaleSuccess = false;
                        inhalePhase = true;
                        exhalePhase = false;
                    }
                }
                if (exhalePhase)
                {
                    if (exhaleDuration >= 1)
                    {
                        inhalePhase = true;
                        exhalePhase = false;
                    }
                }

                // Determine how long the break was
                breakDuration = breakTime - breakStart;
			}

        }
    }

    // Method to receive data message from M5 Stick.
    private void ReceiveSpirometerData(OscMessage message)
    {
        float breathVal = message.GetFloat(0);
		speed = breathVal;
		Debug.Log(breathVal);
        if (breathVal >= exhaleThresh)
        {
            exhaleIsOn = true;
            inhaleIsOn = false;
            breakIsOn = false;
        }

        if (breathVal < exhaleThresh && breathVal > inhaleTresh)
        {
            exhaleIsOn = false;
            inhaleIsOn = false;
            breakIsOn = true;
        }

        if (breathVal <= inhaleTresh)
        {
            inhaleIsOn = true;
            exhaleIsOn = false;
            breakIsOn = false;
        }
    }

    // Determine actions when boat collides with other gameObjects
    private void OnTriggerEnter(Collider other)
    {
        // If it collides with a coin.
        if (other.gameObject.CompareTag("Coin") || other.gameObject.CompareTag("Coin Two"))
        {
            if (exhalePhase)
            {
                Destroy(other.gameObject);
                audio.PlayOneShot(coin, 5f);
                // Update all instances of coinScore so there is data consistency
                coinScores.coinScore += 1;
                treasureScores.coinScore += 1;
                finalScores.coinScore += 1;
                spedometer.coinScore += 1;
            }
        }
        // If it collides with a cloud.
        else if (other.gameObject.CompareTag("Cloud"))
        {
            if (inhalePhase)
            {
                //audio.PlayOneShot(treasure, 3f);
                // Update all instances of treasureScore so there is data consistency
                //coinScores.treasureScore += 1;
                //treasureScores.treasureScore += 1;
                //finalScores.treasureScore += 1;
                //spedometer.treasureScore += 1;
                //Destroy(other.gameObject);
            }
        }
        else if (other.gameObject.CompareTag("Cliff"))
        {
            audio.PlayOneShot(crash, 5f);
            StartCoroutine(BlinkTime(2f));
            if (transform.position.x >= 44)
            {
                transform.Translate(new Vector3(42, transform.position.y, transform.position.z));
            }
            if (transform.position.x <= -44)
            {
                transform.Translate(new Vector3(-42, transform.position.y, transform.position.z));
            }
        } 
        // If it collides with any other object.
        else
        {
            // If the boat collides with an object, blink on and off.
            audio.PlayOneShot(crash, 5f);
            StartCoroutine(BlinkTime(2f));
        }
    }

    // Blink the boat on and off.
    private IEnumerator BlinkTime(float blinkDuration)
    {
        float timeCounter = 0;
        while (timeCounter < blinkDuration)
        {
   //         submarineParts = GameObject.FindGameObjectsWithTag("Submarine");
   //         foreach (GameObject sub in submarineParts)
			//{
   //             Renderer subRender = sub.GetComponent<Renderer>();
   //             subRender.enabled = !subRender.enabled;
			//}
			// make the boat blink off and on.
			gameBoat.enabled = !gameBoat.enabled;
			//wait 0.3 seconds per interval
			yield return new WaitForSeconds(0.3f);
            timeCounter += (1f / 3f);
        }
		gameBoat.enabled = true;
		//foreach (GameObject sub in submarineParts)
  //      {
  //          Renderer subRender = sub.GetComponent<Renderer>();
  //          subRender.enabled = true;
  //      }
	}

    // Only allow player to play when looking in the forward direction.
    private bool cameraBounds()
	{
		if (transform.rotation.eulerAngles.y <= 45 && transform.rotation.eulerAngles.y >= -45)
		{
			return true;
		}
		else
		{
			return false;
		}
	}
}


