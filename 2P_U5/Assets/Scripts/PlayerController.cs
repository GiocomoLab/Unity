using UnityEngine;
using System;
using System.Collections;
using Uniduino;
using System.IO;

//// for sending email when session is done
//using System.Net;
//using System.Net.Mail;
//using System.Net.Security;
//using System.Security.Cryptography.X509Certificates;

public class PlayerController : MonoBehaviour 
{

	public Arduino arduino;
	private Vector3 initialPosition;
	private int numRewards = 0;
	private int numRewards_manual = 0;
	private int rewardFlag = 0;
	private int landmarkFlag = 0;
	public int numTraversals = 0;
	private int percentOfTrialsRewardOmitted = 0;
	public int numTrialsTotal;


	// for saving data
	private string localDirectory;
	private string serverDirectory;
	private SessionParams paramsScript;
	private string mouse;
	private string session;
	private bool saveData;
	
	// lights, for visual landmarks
	public Light light1;
	public Light light2;
	private Color initialBackgroundColor;
	private Color initialAmbientLight;

    private Vector3 movement;

    private GameObject player;
    private GameObject reward;
    private GameObject teleport;
    private GameObject panoCam;
    private GameObject blackCam;

    void Start ()
	{	
		initialPosition = new Vector3 (0f, 0.5f, -30f);
        

        // initialize arduino
        arduino = Arduino.global;
		arduino.Setup(ConfigurePins);

        // turn off lights
        panoCam = GameObject.Find("panoCamera");
        blackCam = GameObject.Find("Black Camera");
        
        StartCoroutine (LightsOff());

		// get session parameters from SessionParams script
		GameObject player = GameObject.Find ("Player");
		paramsScript = player.GetComponent<SessionParams> ();
		numTrialsTotal = paramsScript.numTrialsTotal;
        

        // find reward and teleport objects
        reward = GameObject.Find("Reward");
        teleport = GameObject.Find("Teleport");
        

        // for saving data
        //mouse = paramsScript.mouse;
		//session = paramsScript.session;
		//saveData = paramsScript.saveData;
		//localDirectory = paramsScript.localDirectory;
		//serverDirectory = paramsScript.serverDirectory;
	}

	void ConfigurePins () 
	{
		arduino.pinMode (12, PinMode.OUTPUT);
		arduino.pinMode (13, PinMode.OUTPUT);
		Debug.Log ("Pins configured (player controller)");
	}

	void Update ()
	{

		// make sure rotation angle is 0
		transform.eulerAngles = new Vector3(0.0f, 90.0f, 0.0f);

		if (Input.GetKeyDown (KeyCode.Space)) 
		{
			numRewards_manual += 1;
			Debug.Log (numRewards_manual);
			StartCoroutine( Reward ());
		}

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            StartCoroutine(Punish());
        }
		// end game after appropriate number of trials
		if (transform.position.z <=0 & numTraversals == numTrialsTotal)
		{
			//StartCoroutine (sendEmail ()); // send email notification that session is done
			UnityEditor.EditorApplication.isPlaying = false;
		}

	}

	void OnTriggerEnter (Collider other)
	{
        Debug.Log(other.tag);
		if (other.tag == "Start" && landmarkFlag == 0) {
			if (!GetComponent<AudioSource>().isPlaying) {
				GetComponent<AudioSource>().Play ();
			}
			StartCoroutine (LightsOn());

		}
		else if (other.tag == "Reward") {
            StartCoroutine(Reward());

            movement = new Vector3(0.0f, 0.0f, UnityEngine.Random.Range (5,20));
            reward.transform.position = reward.transform.position + movement;
            teleport.transform.position = teleport.transform.position + movement;
            numRewards += 1;
			Debug.Log ("Rewards: " + numRewards);
			
		}
		else if (other.tag == "Teleport") {
			numTraversals += 1;
			StartCoroutine (LightsOff ());
			transform.position = initialPosition;
            reward.transform.position = new Vector3(0.0f, 0.0f, UnityEngine.Random.Range(5, 20));
            teleport.transform.position = reward.transform.position + new Vector3(0.0f, 0.0f, 10.0f);
		}

	}

	IEnumerator LightsOff ()
	{

        blackCam.SetActive(true);
        panoCam.SetActive(false);
        //light1.enabled = false;
		//light2.enabled = false;
		//RenderSettings.ambientLight = Color.black;
		yield return null;
	}

	IEnumerator LightsOn ()
	{
        panoCam.SetActive(true);
        blackCam.SetActive(false);
		light1.enabled = true;
		light2.enabled = true;
		RenderSettings.ambientLight = initialAmbientLight;
		yield return null;
	}

	IEnumerator Reward ()
	{
		arduino.digitalWrite (12, Arduino.HIGH);
		yield return new WaitForSeconds (0.1f);
		arduino.digitalWrite (12, Arduino.LOW);
	}

    IEnumerator Punish ()
    {
        arduino.digitalWrite(11, Arduino.HIGH);
        yield return new WaitForSeconds(0.1f);
        arduino.digitalWrite(11, Arduino.LOW);
    }

//	IEnumerator sendEmail()
//	{
//		MailMessage mail = new MailMessage ();
//
//		mail.From = new MailAddress ("giocomo.lab.vr.rig@gmail.com");
//		mail.To.Add ("malcgcamp@gmail.com");
//		mail.Subject = "VR session complete";
//		mail.Body = "";
//
//		SmtpClient smtpServer = new SmtpClient ("smtp.gmail.com");
//		smtpServer.Port = 587;
//		smtpServer.Credentials = new System.Net.NetworkCredential ("giocomo.lab.vr.rig", "entorhinal!1234") as ICredentialsByHost;
//		smtpServer.EnableSsl = true;
//		ServicePointManager.ServerCertificateValidationCallback = 
//			delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) {
//			return true;
//		};
//		smtpServer.Send (mail);
//
//		yield return null;
//	}


	// save manipulation data to server
	void OnApplicationQuit ()
	{
		if (saveData) {
			//if (manipSession)
			//{
			//	File.Copy (manipFile,serverManipFile);
			//}
		}
	}

}