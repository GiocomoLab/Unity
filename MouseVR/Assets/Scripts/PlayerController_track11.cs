using UnityEngine;
using System.Collections;
using Uniduino;
using System.IO;

// for sending email when session is done
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

public class PlayerController_track11 : MonoBehaviour 
{
	
	public Arduino arduino;
	public static float distanceTraveled;
	private Vector3 initialPosition;
	private int numRewards = 0;
	private int numRewards_manual = 0;
	private int rewardFlag = 0;
	private int landmarkFlag = 0;
	private static int numTraversals = 0;
	private int percentOfTrialsRewardOmitted = 0;
	public int numTrialsTotal = 200;

	// for manipulation sessions
	public bool manipSession = false;
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
	private Color initialBackgroundColor;
	private Color initialAmbientLight;
	private Camera mainCam;

	void Start ()
	{	
		initialPosition = new Vector3 (0f, 0.5f, -32.25f);
		distanceTraveled = transform.position.z - initialPosition.z;

		// camera
		mainCam = Camera.main;
		initialBackgroundColor = mainCam.backgroundColor;
		initialAmbientLight = RenderSettings.ambientLight;

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
		manipFile = "/Users/malcolmc/Desktop/" + mouse + "/" + session + "_manip_times.txt";
		serverManipFile = "/Volumes/data/Users/MCampbell/" + mouse + "/VR/" + session + "_manip_times.txt";
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
			StartCoroutine( Reward ());
		}

	}

	void OnTriggerEnter (Collider other)
	{
		if (other.tag == "Landmark 1" && landmarkFlag == 0) {
			// landmarkFlag = 1;
			if (!GetComponent<AudioSource>().isPlaying) {
				GetComponent<AudioSource>().Play ();
			}
			StartCoroutine (LightsOn());

		}
		else if (other.tag == "Reward" && rewardFlag == 0) {
			numRewards += 1;
			rewardFlag = 1;
			// Debug.Log ("Rewards: " + numRewards);
			if (Random.Range (1,100) > percentOfTrialsRewardOmitted) {
				StartCoroutine (Reward ());
			}
		}
		else if (other.tag == "Teleport") {
			numTraversals += 1;
			rewardFlag = 0;
			landmarkFlag = 0;
			StartCoroutine (LightsOff ());
			initialPosition = new Vector3(0f,0.5f,430f/400f*-30.0f); // (-30.0f - 1.0f * Random.Range(0,70)));
			transform.position = initialPosition;

			Debug.Log ("Misses: " + numTraversals);
			
			// DO TRIAL TYPE MANIPULATION
			if (manipSession) {
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
		}

	}

	IEnumerator LightsOff ()
	{
		light1.enabled = false;
		light2.enabled = false;
		mainCam.clearFlags = CameraClearFlags.SolidColor;
		mainCam.backgroundColor = Color.black;
		RenderSettings.ambientLight = Color.black;
		yield return null;
	}

	IEnumerator LightsOn ()
	{
		light1.enabled = true;
		light2.enabled = true;
		mainCam.clearFlags = CameraClearFlags.Skybox;
		mainCam.backgroundColor = initialBackgroundColor;
		RenderSettings.ambientLight = initialAmbientLight;
		yield return null;
	}

	IEnumerator LightsOffFor(float sec) {
		StartCoroutine (LightsOff());
		yield return new WaitForSeconds(sec);
		StartCoroutine (LightsOn());
	}
	
	IEnumerator Reward ()
	{
		arduino.digitalWrite (12, Arduino.HIGH);
		yield return new WaitForSeconds (0.075f);
		arduino.digitalWrite (12, Arduino.LOW);
		if (numTraversals == numTrialsTotal) {
			StartCoroutine (sendEmail ()); // send email notification that session is done
			UnityEditor.EditorApplication.isPlaying = false;
		}
	}

	IEnumerator sendEmail()
	{
		MailMessage mail = new MailMessage ();

		mail.From = new MailAddress ("giocomo.lab.vr.rig@gmail.com");
		mail.To.Add ("malcgcamp@gmail.com");
		mail.Subject = "VR session complete";
		mail.Body = "";

		SmtpClient smtpServer = new SmtpClient ("smtp.gmail.com");
		smtpServer.Port = 587;
		smtpServer.Credentials = new System.Net.NetworkCredential ("giocomo.lab.vr.rig", "entorhinal!1234") as ICredentialsByHost;
		smtpServer.EnableSsl = true;
		ServicePointManager.ServerCertificateValidationCallback = 
		delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) {
				return true;
		};
		smtpServer.Send (mail);

		yield return null;
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