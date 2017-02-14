using UnityEngine;
using System.Collections;
using Uniduino;
using System.IO;

public class PlayerController_infiniteTrack : MonoBehaviour 
{

	public float speed;
	public Arduino arduino;
	public static float distanceTraveled;
	private static float initialPosition;
	private int numRewards_towers;
	private int numRewards_manual;

	void Start ()
	{	
		initialPosition = transform.position.z;
		distanceTraveled = transform.position.z - initialPosition;
		numRewards_towers = 0;
		numRewards_manual = 0;

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
		distanceTraveled = transform.position.z - initialPosition;

		// move player based on optical mouse input
		// for now, only move in a straight line
		// float moveHorizontal = Input.GetAxis ("Mouse X");
		float moveVertical = Input.GetAxis ("Mouse Y");
		Vector3 movement = new Vector3 (0.0f, 0.0f, moveVertical);
		transform.position = transform.position + movement * speed * Time.deltaTime;

		if (distanceTraveled > numRewards_towers * 50 + 25) 
		{
			numRewards_towers += 1;
			Debug.Log (numRewards_towers);
			StartCoroutine( Reward ());	
		}

		if (Input.GetKeyDown (KeyCode.Space)) 
		{
			numRewards_manual += 1;
			Debug.Log (numRewards_manual);
			StartCoroutine( Reward ());
		}

	}
	
	IEnumerator Reward ()
	{
		arduino.digitalWrite (12, Arduino.HIGH);
		yield return new WaitForSeconds (0.08f);
		arduino.digitalWrite (12, Arduino.LOW);
	}

	// create behavioral summary on quit
	void OnApplicationQuit ()
	{
		int numRewards = numRewards_towers + numRewards_manual;
		var sw = new StreamWriter ("/Users/malcolmc/Desktop/behavior_summary.txt", true);
		float scaleFactor = (430f/400f)*speed/2.7f;
		float t = Mathf.Round (Time.realtimeSinceStartup);
		float z = Mathf.Round (transform.position.z);
		float real_z = Mathf.Round (transform.position.z / scaleFactor);
		sw.Write ("time(s)\trewards\tdist(vr)\tdist(cm)\n");
		sw.Write (t + "\t" + numRewards + "\t"+ z + "\t" + real_z);
		sw.Close ();
	}

}