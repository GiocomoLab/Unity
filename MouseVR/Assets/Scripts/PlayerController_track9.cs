using UnityEngine;
using System.Collections;
using Uniduino;
using System.IO;

public class PlayerController_track9 : MonoBehaviour 
{

	public float speed;
	public Arduino arduino;
	public static float distanceTraveled;
	private Vector3 initialPosition;
	private int numRewards = 0;
	private int numRewards_manual = 0;
	private int rewardFlag = 0;
	private static int numTraversals = 0;
	private bool switchedRewards = false;
	public bool onlyLastReward = false;

	// for interleaved trial type manipulations
	public bool manipSession = false;
	public bool zenTrack = false;
	public int numTrialsA = 10;
	public int numTrialsB = 5;
	public float speedGain = 1;
	public bool manipTrial = false;

	void Start ()
	{	
		initialPosition = new Vector3 (0f, 0.5f, 0f);
		distanceTraveled = transform.position.z - initialPosition.z;

		// initialize arduino
		arduino = Arduino.global;
		arduino.Setup(ConfigurePins);
	}

	void ConfigurePins () 
	{if (Input.GetKeyDown (KeyCode.F1)) 
		{
			StartCoroutine ("ScreenOff");
		}
		arduino.pinMode (12, PinMode.OUTPUT);
	}

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.UpArrow)) 
		{
			speed = speed*2;
		}
		if (Input.GetKeyDown (KeyCode.DownArrow)) 
		{
			speed = speed/2;	
		}

		distanceTraveled = 430 * numTraversals + (transform.position.z - initialPosition.z);

		// move player based on optical mouse input
		// for now, only move in a straight line
		// float moveHorizontal = Input.GetAxis ("Mouse X");
		float moveVertical = Input.GetAxis ("Mouse Y");
		Vector3 movement = new Vector3 (0.0f, 0.0f, moveVertical);
		transform.position = transform.position + movement * speed * Time.deltaTime;

		if (Input.GetKeyDown (KeyCode.Space)) 
		{
			numRewards_manual += 1;
			Debug.Log (numRewards_manual);
			StartCoroutine( RewardLong ());
		}

		if (Input.GetKeyDown (KeyCode.R)) 
		{
			switchedRewards = true;
		}

		if (Input.GetKeyDown (KeyCode.E)) 
		{
			onlyLastReward = !onlyLastReward;
			Debug.Log("onlyLastReward = " + onlyLastReward);
		}

	}

	void OnTriggerEnter (Collider other)
	{
		if (other.tag == "Reward 1" && (rewardFlag == 0 | switchedRewards)) {
			rewardFlag = 1;
			if (!onlyLastReward)
			{
				StartCoroutine (RewardShort ());
				numRewards += 1;
				Debug.Log ("Rewards: " + numRewards);
			}
			switchedRewards = false;
		}
		else if (other.tag == "Reward 2" && (rewardFlag == 1 | switchedRewards)) {
			rewardFlag = 2;
			if (!onlyLastReward)
			{
				StartCoroutine (RewardShort ());
				numRewards += 1;
				Debug.Log ("Rewards: " + numRewards);
			}

			switchedRewards = false;
		}
		else if (other.tag == "Reward 3" && (rewardFlag == 2 | switchedRewards)) {
			numRewards += 1;
			rewardFlag = 0;
			Debug.Log ("Rewards: " + numRewards);
			StartCoroutine (RewardLong ());
			switchedRewards = false;
		}
		else if (other.tag == "Teleport") {
			numTraversals += 1;
			transform.position = initialPosition;
			Debug.Log ("Traversals: " + numTraversals);

			// DO TRIAL TYPE MANIPULATION
			if (manipSession)
			{
				if (numTraversals == numTrialsA)
				{
					speed = speed * speedGain;
					manipTrial = true;

					// write time to file
					var sw = new StreamWriter ("/Users/malcolmc/Desktop/manip_times.txt", true);
					sw.Write (Time.realtimeSinceStartup + "\n");
					sw.Close ();
				}
				else if (numTraversals == numTrialsA + numTrialsB)
				{
					speed = speed/speedGain;
					manipTrial = false;

					// write time to file
					var sw = new StreamWriter ("/Users/malcolmc/Desktop/manip_times.txt", true);
					sw.Write (Time.realtimeSinceStartup + "\n");
					sw.Close ();
				}
				//var sw = new StreamWriter ("/Users/malcolmc/Desktop/trial_type.txt", true);
				//sw.Write (Time.realtimeSinceStartup + "\t" + manipTrial + "\n");
				//sw.Close ();
				//Debug.Log ("Manip: " + manipTrial);
			}
		}

	}

	IEnumerator RewardShort ()
	{
		arduino.digitalWrite (12, Arduino.HIGH);
		yield return new WaitForSeconds (0.1f);
		arduino.digitalWrite (12, Arduino.LOW);
	}

	IEnumerator RewardLong ()
	{
		arduino.digitalWrite (12, Arduino.HIGH);
		yield return new WaitForSeconds (0.2f);
		arduino.digitalWrite (12, Arduino.LOW);
	}

	// create behavioral summary on quit
	void OnApplicationQuit ()
	{
		var sw = new StreamWriter ("/Users/malcolmc/Desktop/behavior_summary.txt", true);
		float scaleFactor = (430f/400f) * (speed/2.7f);
		float t = Mathf.Round (Time.realtimeSinceStartup);
		float z = Mathf.Round (distanceTraveled);
		float real_z = Mathf.Round (distanceTraveled / scaleFactor);
		sw.Write ("time(s)\trewards\ttraversals\tdist(vr)\tdist(cm)\n");
		sw.Write (t + "\t" + numRewards + "\t" + numTraversals + "\t"+ z + "\t" + real_z);
		sw.Close ();
	}

}