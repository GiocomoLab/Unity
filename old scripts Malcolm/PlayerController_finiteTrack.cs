using UnityEngine;
using System.Collections;
using Uniduino;
using System.IO;

public class PlayerController_finiteTrack : MonoBehaviour 
{

	public float speed;
	public Arduino arduino;
	public static float distanceTraveled;
	private Vector3 initialPosition;
	private int numRewards;
	private int rewardFlag;
	private int numTraversals;

	void Start ()
	{	
		initialPosition = transform.position;
		distanceTraveled = transform.position.z - initialPosition.z;
		numRewards = 0;
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
		distanceTraveled = 215 * numTraversals + (transform.position.z - initialPosition.z);

		// move player based on optical mouse input
		// for now, only move in a straight line
		// float moveHorizontal = Input.GetAxis ("Mouse X");
		float moveVertical = Input.GetAxis ("Mouse Y");
		Vector3 movement = new Vector3 (0.0f, 0.0f, moveVertical);
		transform.position = transform.position + movement * speed * Time.deltaTime;

	}

	void OnTriggerEnter (Collider other)
	{
		if (other.tag == "Reward 1" && rewardFlag == 0) {
			Debug.Log ("Entered Reward 1 zone");
			numRewards += 1;
			rewardFlag = 1;
			Debug.Log (numRewards);
			StartCoroutine (Reward ());
		}
		else if (other.tag == "Reward 2" && rewardFlag == 1) {
			numRewards += 1;
			rewardFlag = 0;
			Debug.Log (numRewards);
			StartCoroutine (Reward ());
		}
		else if (other.tag == "Teleport") {
			numTraversals += 1;
			transform.position = initialPosition;
		}

	}

	IEnumerator Reward ()
	{
		arduino.digitalWrite (12, Arduino.HIGH);
		yield return new WaitForSeconds (0.5f);
		arduino.digitalWrite (12, Arduino.LOW);
	}

	// create behavioral summary on quit
	void OnApplicationQuit ()
	{
		var sw = new StreamWriter ("/Users/malcolmc/Desktop/behavior_summary.txt", true);
		float scaleFactor = 1.08273f;
		float t = Mathf.Round (Time.realtimeSinceStartup);
		float z = Mathf.Round (distanceTraveled);
		float real_z = Mathf.Round (distanceTraveled / scaleFactor);
		sw.Write ("time(s)\trewards\ttraversals\tdist(vr)\tdist(cm)\n");
		sw.Write (t + "\t" + numRewards + "\t" + numTraversals + "\t"+ z + "\t" + real_z);
		sw.Close ();
	}

}