using UnityEngine;
using System.Collections;
using Uniduino;
using System.IO;

public class PlayerController_finiteTrack3 : MonoBehaviour 
{

	public float speed;
	public Arduino arduino;
	public static float distanceTraveled;
	public bool onlySecondReward = false;
	private Vector3 initialPosition;
	private int numRewards;
	private int numRewards_manual;
	private int rewardFlag;
	private int numTraversals;
	private bool stopRewards;

	void Start ()
	{	
		initialPosition = new Vector3(0f,0.5f,0f);
		distanceTraveled = transform.position.z - initialPosition.z;
		numRewards = 0;
		numRewards_manual = 0;
		rewardFlag = 0;
		numTraversals = 0;
		stopRewards = false;

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
			var sw = new StreamWriter ("/Users/malcolmc/Desktop/manip.txt", true);
			sw.Write (Time.realtimeSinceStartup + "\n");
			sw.Close ();
		}
		if (Input.GetKeyDown (KeyCode.DownArrow)) 
		{
			speed = speed/2;
			var sw = new StreamWriter ("/Users/malcolmc/Desktop/manip.txt", true);
			sw.Write (Time.realtimeSinceStartup + "\n");
			sw.Close ();
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
			StartCoroutine( Reward ());
		}
		if (Input.GetKeyDown (KeyCode.Q)) 
		{
			stopRewards = !stopRewards;
		}
		if (Input.GetKeyDown (KeyCode.W)) 
		{
			onlySecondReward = !onlySecondReward;
			var sw = new StreamWriter ("/Users/malcolmc/Desktop/manip.txt", true);
			sw.Write (Time.realtimeSinceStartup + "\n");
			sw.Close ();
		}

	}

	void OnTriggerEnter (Collider other)
	{
		if ((other.tag == "Reward 1" && rewardFlag == 0) & !stopRewards)  {
			numRewards += 1;
			rewardFlag = 1;
			Debug.Log ("Rewards: " + numRewards);
			if (onlySecondReward == false)
			{
				StartCoroutine (Reward ());
			}
		}
		else if ((other.tag == "Reward 2" && rewardFlag == 1) & !stopRewards) {
			numRewards += 1;
			rewardFlag = 0;
			Debug.Log ("Rewards: " + numRewards);
			StartCoroutine (Reward ());
		}
		else if (other.tag == "Teleport") {
			numTraversals += 1;
			Debug.Log("Traversals: " + numTraversals);
			transform.position = initialPosition;
		}

	}

	IEnumerator Reward ()
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