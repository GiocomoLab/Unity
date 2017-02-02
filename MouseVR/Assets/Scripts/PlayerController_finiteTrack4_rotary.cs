using UnityEngine;
using System.Collections;
using Uniduino;
using System.IO;

public class PlayerController_finiteTrack4_rotary : MonoBehaviour 
{

	public Arduino arduino;
	public static float distanceTraveled;
	private Vector3 initialPosition;
	private int numRewards;
	private int numRewards_manual;
	private int rewardFlag;
	private int numTraversals;
	
	void Start ()
	{	
		initialPosition = new Vector3 (0f, 0.5f, 0f);
		distanceTraveled = transform.position.z - initialPosition.z;
		numRewards = 0;
		numRewards_manual = 0;
		rewardFlag = 0;
		numTraversals = 0;
		
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
		distanceTraveled = 430 * numTraversals + (transform.position.z - initialPosition.z);

		if (Input.GetKeyDown (KeyCode.Space)) 
		{
			numRewards_manual += 1;
			Debug.Log (numRewards_manual);
			StartCoroutine( RewardLong ());
		}
		
	}
	
	void OnTriggerEnter (Collider other)
	{
		if (other.tag == "Reward 1" && rewardFlag == 0) {
			Debug.Log ("Entered Reward 1 zone");
			numRewards += 1;
			rewardFlag = 1;
			Debug.Log (numRewards);
			StartCoroutine (RewardShort ());
		}
		else if (other.tag == "Reward 2" && rewardFlag == 1) {
			numRewards += 1;
			rewardFlag = 2;
			Debug.Log (numRewards);
			StartCoroutine (RewardShort ());
		}
		else if (other.tag == "Reward 3" && rewardFlag == 2) {
			numRewards += 1;
			rewardFlag = 0;
			Debug.Log (numRewards);
			StartCoroutine (RewardLong ());
		}
		else if (other.tag == "Teleport") {
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
		float scaleFactor = 1.211169f;
		float t = Mathf.Round (Time.realtimeSinceStartup);
		float z = Mathf.Round (distanceTraveled);
		float real_z = Mathf.Round (distanceTraveled / scaleFactor);
		sw.Write ("time(s)\trewards\ttraversals\tdist(vr)\tdist(cm)\n");
		sw.Write (t + "\t" + numRewards + "\t" + numTraversals + "\t"+ z + "\t" + real_z);
		sw.Close ();
	}
	
}