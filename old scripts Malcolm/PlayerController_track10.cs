using UnityEngine;
using System.Collections;
using Uniduino;
using System.IO;

public class PlayerController_track10 : MonoBehaviour 
{
	
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

	// for saving data
	private SessionParams paramsScript;
	private string mouse;
	private string session;
	private bool saveData;
	private string summaryFile;
	private string serverSummaryFile;

	void Start ()
	{	
		initialPosition = new Vector3 (0f, 0.5f, 0f);
		distanceTraveled = transform.position.z - initialPosition.z;

		// initialize arduino
		arduino = Arduino.global;
		arduino.Setup(ConfigurePins);

		// for saving data
		GameObject player = GameObject.Find ("Player");
		paramsScript = player.GetComponent<SessionParams> ();
		mouse = paramsScript.mouse;
		session = paramsScript.session;
		saveData = paramsScript.saveData;
		summaryFile = "/Users/malcolmc/Desktop/" + mouse + "/" + session + "_behavior_summary.txt";
		serverSummaryFile = "/Volumes/data/Users/MCampbell/" + mouse + "/VR/" + session + "_behavior_summary.txt";
	}

	void ConfigurePins () 
	{
		arduino.pinMode (12, PinMode.OUTPUT);
	}

	void Update ()
	{

		distanceTraveled = 300 * numTraversals + (transform.position.z - initialPosition.z);

		if (Input.GetKeyDown (KeyCode.A)) 
		{
			automaticRewards = !automaticRewards;
		}

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
			if (other.tag == "Reward 2" && (rewardFlag == 0 | switchedRewards)) {
				rewardFlag = 1;
				if (!onlyLastReward)
				{
					StartCoroutine (RewardLong ());
					numRewards += 1;
				}
				Debug.Log (numRewards);
			}
		}
		if (other.tag == "Teleport") {
			numTraversals += 1;
			rewardFlag = 0;
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
		if (saveData)
		{
			var sw = new StreamWriter (summaryFile, true);
			float scaleFactor = (430f/400f);
			float t = Mathf.Round (Time.realtimeSinceStartup);
			float z = Mathf.Round (distanceTraveled);
			float real_z = Mathf.Round (distanceTraveled / scaleFactor);
			sw.Write ("time(s)\trewards\ttraversals\tdist(vr)\tdist(cm)\n");
			sw.Write (t + "\t" + numRewards + "\t" + numTraversals + "\t"+ z + "\t" + real_z);
			sw.Close ();
			File.Copy (summaryFile, serverSummaryFile);
		}
	}

}