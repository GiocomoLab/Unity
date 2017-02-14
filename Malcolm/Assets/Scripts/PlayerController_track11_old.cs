using UnityEngine;
using System.Collections;
using Uniduino;
using System.IO;

public class PlayerController_track11_old : MonoBehaviour 
{
	
	public Arduino arduino;
	public static float distanceTraveled;
	private Vector3 initialPosition;
	private int numRewards = 0;
	private int numRewards_manual = 0;
	private int rewardFlag = 0;
	private int soundFlag = 0;
	private static int numTraversals = 0;
	private bool switchedRewards = false;
	private bool onlyLastReward = true;
	private bool variablePause = false;
	private int percentOfTrialsRewardOmitted = 0;
	private Camera mainCam;

	// for manipulation sessions
	public bool manipSession = false;
	public bool interleavedTrials = false;
	public bool testRangeOfGains = false;
	public bool zenTrack = false;
	public bool flipStripes = false;
	public bool removeTowers = false;
	public int numTrialsA = 10;
	public int numTrialsB = 5;
	public float speedGain = 1f;
	public bool manipTrial = false;
	private int numNormalTrials_RangeOfGains = 20;

	// for saving data
	private SessionParams paramsScript;
	private string mouse;
	private string session;
	private bool saveData;
	private string summaryFile;
	private string serverSummaryFile;
	private string manipFile;
	private string serverManipFile;

	// lights, for visual landmarks
	public Light light1;
	public Light light2;
	public Light light3;

	void Start ()
	{	
		initialPosition = new Vector3 (0f, 0.5f, 0f);
		distanceTraveled = transform.position.z - initialPosition.z;

		// camera
		mainCam = Camera.main;

		// initialize arduino
		arduino = Arduino.global;
		Debug.Log ("Got here");
		arduino.Setup(ConfigurePins);

		// for saving data
		GameObject player = GameObject.Find ("Player");
		paramsScript = player.GetComponent<SessionParams> ();
		mouse = paramsScript.mouse;
		session = paramsScript.session;
		saveData = paramsScript.saveData;
		summaryFile = "/Users/malcolmc/Desktop/" + mouse + "/" + session + "_behavior_summary.txt";
		serverSummaryFile = "/Volumes/data/Users/MCampbell/" + mouse + "/VR/" + session + "_behavior_summary.txt";
		if (interleavedTrials) {
			manipFile = "/Users/malcolmc/Desktop/" + mouse + "/" + session + "_trial_type.txt";
			serverManipFile = "/Volumes/data/Users/MCampbell/" + mouse + "/VR/" + session + "_trial_type.txt";
		} else {
			manipFile = "/Users/malcolmc/Desktop/" + mouse + "/" + session + "_manip_times.txt";
			serverManipFile = "/Volumes/data/Users/MCampbell/" + mouse + "/VR/" + session + "_manip_times.txt";
		}
	}

	void ConfigurePins () 
	{
		arduino.pinMode (12, PinMode.OUTPUT);
		Debug.Log ("Pins configured");
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
		if (other.tag == "Sound 1" && soundFlag == 0) {
			soundFlag = 1;
			if (!GetComponent<AudioSource>().isPlaying) {
				GetComponent<AudioSource>().Play ();
			}
			StartCoroutine (LightsOn());

		}
		if (other.tag == "Sound 2" && soundFlag == 1) {
			soundFlag = 0;
			if (!GetComponent<AudioSource>().isPlaying) {
				GetComponent<AudioSource>().Play ();
			}
			StartCoroutine (LightsOffFor (0.3f));
		}
		else if (other.tag == "Reward 3" && rewardFlag == 0) {
			numRewards += 1;
			rewardFlag = 1;
			Debug.Log ("Rewards: " + numRewards);
			if (Random.Range (1,100) > percentOfTrialsRewardOmitted) {
				StartCoroutine (RewardLong ());
			}
		}
		else if (other.tag == "Teleport") {
			numTraversals += 1;
			rewardFlag = 0;
			StartCoroutine (LightsOff ());
			if (variablePause) 
			{
				int randomNumber = Random.Range (-100,100);
				float p = 2f + randomNumber/50f;
				Debug.Log (p);
				StartCoroutine (TeleportWithPause(p));
			} else { 
				initialPosition = new Vector3(0f,0.5f,0f - 430f/400f * Random.Range(0,20));
				transform.position = initialPosition;
			}

			Debug.Log ("Traversals: " + numTraversals);
			
			// DO TRIAL TYPE MANIPULATION
			if (manipSession) {
				if (!interleavedTrials)
				{
					if (numTraversals == numTrialsA) {
						//speed = speed * speedGain;
						manipTrial = true;
						
						// write time to file
						var sw = new StreamWriter (manipFile, true);
						sw.Write (Time.realtimeSinceStartup + "\n");
						sw.Close ();
					} else if (numTraversals == numTrialsA + numTrialsB) {
						//speed = speed / speedGain;
						manipTrial = false;
						
						// write time to file
						var sw = new StreamWriter (manipFile, true);
						sw.Write (Time.realtimeSinceStartup + "\n");
						sw.Close ();
					}
				}
				else {
					if ( (numTraversals+1) % (numTrialsA+numTrialsB) == 0)
					{
						manipTrial = true;
					} else { 
						manipTrial = false;
					}
					var sw = new StreamWriter (manipFile, true);
					sw.Write (Time.realtimeSinceStartup + "\t" + manipTrial + "\n");
					sw.Close ();
				}
			} else if (testRangeOfGains) {
				if(numTraversals == numNormalTrials_RangeOfGains) {
					manipTrial = true;
					speedGain = 0.9f;
					var sw = new StreamWriter (manipFile, true);
					sw.Write (Time.realtimeSinceStartup + "\n");
					sw.Close ();
				} else if (numTraversals == numNormalTrials_RangeOfGains+1) {
					manipTrial = true;
					speedGain = 0.8f;
					var sw = new StreamWriter (manipFile, true);
					sw.Write (Time.realtimeSinceStartup + "\n");
					sw.Close ();
				} else if (numTraversals == numNormalTrials_RangeOfGains+2) {
					manipTrial = true;
					speedGain = 0.7f;
					var sw = new StreamWriter (manipFile, true);
					sw.Write (Time.realtimeSinceStartup + "\n");
					sw.Close ();
				} else if (numTraversals == numNormalTrials_RangeOfGains+3) {
					manipTrial = true;
					speedGain = 0.6f;
					var sw = new StreamWriter (manipFile, true);
					sw.Write (Time.realtimeSinceStartup + "\n");
					sw.Close ();
				} else if (numTraversals == numNormalTrials_RangeOfGains+4) {
					manipTrial = true;
					speedGain = 0.5f;
					var sw = new StreamWriter (manipFile, true);
					sw.Write (Time.realtimeSinceStartup + "\n");
					sw.Close ();
				} else if (numTraversals == numNormalTrials_RangeOfGains+5) {
					manipTrial = true;
					speedGain = 0.6f;
					var sw = new StreamWriter (manipFile, true);
					sw.Write (Time.realtimeSinceStartup + "\n");
					sw.Close ();
				} else if (numTraversals == numNormalTrials_RangeOfGains+6) {
					manipTrial = true;
					speedGain = 0.7f;
					var sw = new StreamWriter (manipFile, true);
					sw.Write (Time.realtimeSinceStartup + "\n");
					sw.Close ();
				} else if (numTraversals == numNormalTrials_RangeOfGains+7) {
					manipTrial = true;
					speedGain = 0.8f;
					var sw = new StreamWriter (manipFile, true);
					sw.Write (Time.realtimeSinceStartup + "\n");
					sw.Close ();
				} else if (numTraversals == numNormalTrials_RangeOfGains+8) {
					manipTrial = true;
					speedGain = 0.9f;
					var sw = new StreamWriter (manipFile, true);
					sw.Write (Time.realtimeSinceStartup + "\n");
					sw.Close ();
				} else if (numTraversals == 2*numNormalTrials_RangeOfGains+9) {
					manipTrial = true;
					speedGain = 1.1f;
					var sw = new StreamWriter (manipFile, true);
					sw.Write (Time.realtimeSinceStartup + "\n");
					sw.Close ();
				} else if (numTraversals == 2*numNormalTrials_RangeOfGains+10) {
					manipTrial = true;
					speedGain = 1.2f;
					var sw = new StreamWriter (manipFile, true);
					sw.Write (Time.realtimeSinceStartup + "\n");
					sw.Close ();
				} else if (numTraversals == 2*numNormalTrials_RangeOfGains+11) {
					manipTrial = true;
					speedGain = 1.3f;
					var sw = new StreamWriter (manipFile, true);
					sw.Write (Time.realtimeSinceStartup + "\n");
					sw.Close ();
				} else if (numTraversals == 2*numNormalTrials_RangeOfGains+12) {
					manipTrial = true;
					speedGain = 1.4f;
					var sw = new StreamWriter (manipFile, true);
					sw.Write (Time.realtimeSinceStartup + "\n");
					sw.Close ();
				} else if (numTraversals == 2*numNormalTrials_RangeOfGains+13) {
					manipTrial = true;
					speedGain = 1.5f;
					var sw = new StreamWriter (manipFile, true);
					sw.Write (Time.realtimeSinceStartup + "\n");
					sw.Close ();
				} else if (numTraversals == 2*numNormalTrials_RangeOfGains+14) {
					manipTrial = true;
					speedGain = 1.4f;
					var sw = new StreamWriter (manipFile, true);
					sw.Write (Time.realtimeSinceStartup + "\n");
					sw.Close ();
				} else if (numTraversals == 2*numNormalTrials_RangeOfGains+15) {
					manipTrial = true;
					speedGain = 1.3f;
					var sw = new StreamWriter (manipFile, true);
					sw.Write (Time.realtimeSinceStartup + "\n");
					sw.Close ();
				} else if (numTraversals == 2*numNormalTrials_RangeOfGains+16) {
					manipTrial = true;
					speedGain = 1.2f;
					var sw = new StreamWriter (manipFile, true);
					sw.Write (Time.realtimeSinceStartup + "\n");
					sw.Close ();
				} else if (numTraversals == 2*numNormalTrials_RangeOfGains+17) {
					manipTrial = true;
					speedGain = 1.1f;
					var sw = new StreamWriter (manipFile, true);
					sw.Write (Time.realtimeSinceStartup + "\n");
					sw.Close ();
				} else {
					manipTrial = false;
					speedGain = 1f;
					var sw = new StreamWriter (manipFile, true);
					sw.Write (Time.realtimeSinceStartup + "\n");
					sw.Close ();
				}
			}
		}

	}

	IEnumerator LightsOff ()
	{
		light1.enabled = false;
		light2.enabled = false;
		light3.enabled = false;
		yield return null;
	}

	IEnumerator LightsOn ()
	{
		light1.enabled = true;
		light2.enabled = true;
		light3.enabled = true;
		yield return null;
	}

	IEnumerator LightsOffFor(float sec) {
		StartCoroutine (LightsOff());
		yield return new WaitForSeconds(sec);
		StartCoroutine (LightsOn());
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

	IEnumerator TeleportWithPause(float p)
	{
		Debug.Log ("Got here.");
		yield return new WaitForSeconds(p);
		transform.position = initialPosition;
	}

	// create behavioral summary on quit
	void OnApplicationQuit ()
	{
		if (saveData) {
			var sw = new StreamWriter (summaryFile, true);
			float scaleFactor = (429f / 400f);
			float t = Mathf.Round (Time.realtimeSinceStartup);
			float real_z = Mathf.Round (distanceTraveled / scaleFactor);
			float avgSpeed = real_z/t;
			sw.Write ("time(s)\trewards\ttraversals\tdist(cm)\tavgSpeed(cm/s)\n");
			sw.Write (t + "\t" + numRewards + "\t" + numTraversals + "\t" + real_z + "\t" + avgSpeed);
			sw.Close ();
			File.Copy (summaryFile, serverSummaryFile);
			if (manipSession)
			{
				File.Copy (manipFile,serverManipFile);
			}
		}
	}

}