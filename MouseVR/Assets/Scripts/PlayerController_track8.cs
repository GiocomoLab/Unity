using UnityEngine;
using System.Collections;
using Uniduino;
using System.IO;

public class PlayerController_track8 : MonoBehaviour 
{

	public float speed;
	public Arduino arduino;
	public static float distanceTraveled;
	private Vector3 initialPosition;
	private int numRewards = 0;
	private int numRewards_manual = 0;
	private int rewardFlag = 0;
	private int numTraversals = 0;
	private bool switchedRewards = false;
	public bool onlyLastReward = false;
	public bool automaticRewards = true;

	void Start ()
	{	
		initialPosition = new Vector3 (0f, 0.5f, 0f);
		distanceTraveled = transform.position.z - initialPosition.z;

		// initialize arduino
		arduino = Arduino.global;
		arduino.Setup(ConfigurePins);
	}

	void ConfigurePins () 
	{
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
		if (Input.GetKeyDown (KeyCode.A)) 
		{
			automaticRewards = !automaticRewards;
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
		if (automaticRewards)
		{
			if (other.tag == "Reward 1" && (rewardFlag == 0 | switchedRewards)) {
				rewardFlag = 1;
				if (!onlyLastReward)
				{
					StartCoroutine (RewardShort ());
					numRewards += 1;
				}
				Debug.Log (numRewards);
				switchedRewards = false;
			}
			else if (other.tag == "Reward 2" && (rewardFlag == 1 | switchedRewards)) {
				rewardFlag = 2;
				if (!onlyLastReward)
				{
					StartCoroutine (RewardShort ());
					numRewards += 1;
				}
				Debug.Log (numRewards);
				switchedRewards = false;
			}
			else if (other.tag == "Reward 3" && (rewardFlag == 2 | switchedRewards)) {
				numRewards += 1;
				rewardFlag = 0;
				Debug.Log (numRewards);
				StartCoroutine (RewardLong ());
				switchedRewards = false;
			}
		}
		if (other.tag == "Teleport") {
			numTraversals += 1;
			transform.position = initialPosition;
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
		float scaleFactor = (430f/400f) * (speed/3.23f);
		float t = Mathf.Round (Time.realtimeSinceStartup);
		float z = Mathf.Round (distanceTraveled);
		float real_z = Mathf.Round (distanceTraveled / scaleFactor);
		sw.Write ("time(s)\trewards\ttraversals\tdist(vr)\tdist(cm)\n");
		sw.Write (t + "\t" + numRewards + "\t" + numTraversals + "\t"+ z + "\t" + real_z);
		sw.Close ();
	}

}