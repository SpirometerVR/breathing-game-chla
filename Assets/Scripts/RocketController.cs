using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketController : MonoBehaviour
{
    // Flags for exhale and inhale phases.
    public bool exhalePhase = false;
    public bool inhalePhase = true;

    public GameObject miniDiamond;
    public GameObject miniDiamondTwo;

    public bool inBounds;

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

    // Debugging the spirometer
    public float speed;

    // Music that will be played when items are collected.
    public AudioClip diamond;
    public AudioClip crash;
    public AudioClip fuel;

    // Variables to track inhale & exhale duration.
    private float downTime = 0f;
    private float upTime = 0f;
	private float breakTime = 0f;
	private float exhaleStart = 0f;
    private float inhaleStart = 0f;
	private float breakStart = 0f;

	public bool exhaleIsOn = false;
    public bool inhaleIsOn = false;
    public bool breakIsOn = false;

    // Target threshold values for inhale and exhale.
    private float exhaleThresh = 1470f;
    private float inhaleTresh = 1200f;
    private float steadyThresh = 1340f;
    private float speedMultiplier = 4f;

    private AudioSource audio;
    private Renderer gameRocket;

    // Create GameObject to find OSC
    private GameObject OSC;
    // Hold OSC data in spirometer object
    private OSC spirometer;
    // Get the rocket as a rigidbody
    private Rigidbody rocketBody;

    private ScoreBoard diamondScores;
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

        // Get game renderer for the Rocket
        gameRocket = GetComponent<Renderer>();
        gameRocket.enabled = true;

        // Get rigid body and audio components for the rocket.
        rocketBody = GetComponent<Rigidbody>();
        audio = GetComponent<AudioSource>();

        // Find the score board objects for each respective scoreboard.
        diamondScores = GameObject.FindGameObjectWithTag("Diamond Score").GetComponent<ScoreBoard>();
        finalScores = GameObject.FindGameObjectWithTag("Final Score").GetComponent<ScoreBoard>();
		spedometer = GameObject.FindGameObjectWithTag("Spedometer").GetComponent<ScoreBoard>();

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
            Destroy(GameObject.FindGameObjectWithTag("Right Fuel"));
            Destroy(GameObject.FindGameObjectWithTag("Left Fuel"));
            Destroy(GameObject.FindGameObjectWithTag("Middle Fuel"));
        }
        // Otherwise, if the game is not over:
        if (!gameOver)
        {
			// Unfreeze restrictions so that ship moves normally when not in collision mode.
			rocketBody.constraints = RigidbodyConstraints.None;
			rocketBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY;
			rocketBody.isKinematic = false;

			// Change rocket direction based on camera in VR.
			transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, Camera.main.transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
            Vector3 cameraVector = new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z);

            // Accelerate rocket when player is exhaling or using upArrow input.
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
                    // Use transform.translate so that space ship does not stop on collisions.
                    transform.Translate(new Vector3(cameraVector.x, 0, cameraVector.z) * 300 * Time.deltaTime);
                    //rocketBody.AddRelativeForce(new Vector3(cameraVector.x, 0, cameraVector.z) * speedMultiplier, ForceMode.VelocityChange);
                    // Determine how long the exhale is or how long upArrow is being held down for.
                    exhaleDuration = downTime - exhaleStart;
					// Start counting the break time
					breakStart = Time.time;
				}

				//TO ALLOW KEY BOARD PLAYABILITY, UNCOMMENT IF STATEMENT BELOW:
				if (!Input.GetKey(KeyCode.UpArrow))
				{
					exhaleIsOn = false;
				}
			}

            if (inhalePhase && cameraBounds())
            {
                // Pull fuel towards the rocket when inhaling or using Space key.
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
                }

				//TO ALLOW KEY BOARD PLAYABILITY, UNCOMMENT IF LOOP BELOW:
				if (!Input.GetKey(KeyCode.Space))
				{
					inhaleIsOn = false;
				}
			}

            // If the player is neither exhaling nor inhaling:
            if (!exhaleIsOn && !inhaleIsOn)
            {
                // Repeat code for controller input. This needs to be done so that there are no issues with VR input.
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

                    // Let the spaceship float for a duration of time. Do not do this on start cycle.
                    if (breakDuration <= 0.3f && exhaleDuration > 0)
                    {
                        rocketBody.AddRelativeForce(new Vector3(cameraVector.x, 0, cameraVector.z) * 4f, ForceMode.VelocityChange);
                    }
                    // Negate the force added to the rocket via exhalation.
                    else
                    {
                        var oppositeDirX = -rocketBody.velocity;
                        rocketBody.AddForce(oppositeDirX);
                    }

                    // Only count exhale and inhales that are longer than 0.3 second to remove erroneous air flow data.
                    // Once inhale or exhale is conducted and completed, switch cycles.
                    if (inhalePhase && inhaleDuration >= 0.3)
                    {
                            inhalePhase = false;
                            exhalePhase = true;
                    }
                    if (exhalePhase && exhaleDuration >= 0.3)
                    {
                            inhalePhase = true;
                            exhalePhase = false;
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

                // Let the spaceship float for a duration of time.
                if (breakDuration <= 1f && exhaleDuration > 0)
                {
                    rocketBody.AddRelativeForce(new Vector3(cameraVector.x, 0, cameraVector.z) * 1, ForceMode.VelocityChange);
                }
                // Negate the force added to the rocket via exhalation.
                else
                {
                    var oppositeDir = -rocketBody.velocity;
                    rocketBody.AddForce(oppositeDir);
                }

                // Only count exhale and inhales that are longer than 0.3 second to remove erroneous air flow data.
                // Once inhale or exhale is conducted and completed, switch cycles.
                if (inhalePhase && inhaleDuration >= 0.3)
                {
                    inhalePhase = false;
                    exhalePhase = true;
                }
                if (exhalePhase && exhaleDuration >= 0.3)
                {
                    inhalePhase = true;
                    exhalePhase = false;
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
		// Debugging purposes.
		speed = breathVal;
		Debug.Log(transform.rotation.eulerAngles.y);

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

    // Determine actions when rocket collides with other gameObjects
    private void OnTriggerEnter(Collider other)
    {
        // If it collides with a diamond.
        if (other.gameObject.CompareTag("Diamond") || other.gameObject.CompareTag("Diamond Two"))
        {
            if (exhalePhase)
            {
                // Add constraints so that ship does not float randomly on collision
                rocketBody.constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionX;
				rocketBody.isKinematic = true;
				transform.rotation = Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y, 0);

                Destroy(other.gameObject);
                audio.PlayOneShot(diamond, 5f);

                // Create mini diamonds for score UI. See DiamondController for controller script.
                Instantiate(miniDiamond, new Vector3(transform.position.x, transform.position.y + 10, transform.position.z), Quaternion.Euler(90, 180, 0));

                // Update all instances of diamondScore so there is data consistency
                finalScores.diamondScore += 1;
                spedometer.diamondScore += 1;
            }
        }
        // If it collides with fuel.
        else if (other.gameObject.CompareTag("Fuel"))
        {
            if (inhalePhase)
            {
                // Add constraints so that ship does not float randomly on collision
                rocketBody.constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionX;
				rocketBody.isKinematic = true;
				transform.rotation = Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y, 0);
            }
        }
        // If it collides with any other object.
        else
        {
			// Add constraints so that ship does not float randomly on collision
			rocketBody.constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionX;
			rocketBody.isKinematic = true;
			transform.rotation = Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y, 0);

            Destroy(other.gameObject);
            audio.PlayOneShot(crash, 0.5f);
            // Blink ship on crash.
            StartCoroutine(BlinkTime(2f));
            // Create mini diamonds for score UI. See DiamondFall for controller script.
            GameObject board = GameObject.FindGameObjectWithTag("Diamond Score");
            // Do not let score be negative.
            if (diamondScores.diamondScore >= 1)
            {
                Instantiate(miniDiamondTwo, new Vector3(board.transform.position.x, board.transform.position.y - 3, board.transform.position.z), Quaternion.Euler(90, 180, 0));
            }
        } 
    }

    // Blink the rocket on and off.
    private IEnumerator BlinkTime(float blinkDuration)
    {
        float timeCounter = 0;
        while (timeCounter < blinkDuration)
        {
            // make the rocket blink off and on.
            gameRocket.enabled = !gameRocket.enabled;
            //wait 0.3 seconds per interval
            yield return new WaitForSeconds(0.3f);
            timeCounter += (1f / 3f);
        }
        gameRocket.enabled = true;
    }

	// Only allow player to play when looking in the forward direction.
	private bool cameraBounds()
	{
		if ((transform.rotation.eulerAngles.y >= 0 && transform.rotation.eulerAngles.y <= 60) || (transform.rotation.eulerAngles.y >= 300 && transform.rotation.eulerAngles.y <= 360))
        {
			inBounds = true;
		}
		else
		{
			inBounds = false;
		}
        return inBounds;
	}
}


