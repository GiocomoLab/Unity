using UnityEngine;
using System.Collections;
using System;
using System.IO;
using Uniduino;

// for sending email when session is done
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

public class DetectLicks_shaping : MonoBehaviour {

	public Arduino arduino;

	public int pin = 0;
	public int pinValue;
	private int numLicks = 0;
	private int lickFlag = 0;

	// for saving data
	private SessionParams paramsScript;
	private string mouse;
	private string session;
	private bool saveData;
	private string lickFile;
	private string serverLickFile;


	// shaping variables
	public bool punish = false;
	public int numHitsTotal = 200;
	public float pos1 = 75.25f;
	public float pos2 = 96.75f;
	private Vector3 punishPosition = new Vector3 (0f, 0.5f, 430f / 400f * -200.0f);
	private Vector3 initialPosition = new Vector3 (0f, 0.5f, 430f / 400f * -30.0f);
	private int numHits = 0;
	private int numMisses = 0;

	void Start( )
	{
		arduino = Arduino.global;
		arduino.Setup(ConfigurePins);

		// for saving data
		GameObject player = GameObject.Find ("Player");
		paramsScript = player.GetComponent<SessionParams> ();
		mouse = paramsScript.mouse;
		session = paramsScript.session;
		saveData = paramsScript.saveData;
		lickFile = "/Users/malcolmc/Desktop/" + mouse + "/" + session + "_licks.txt";
		serverLickFile = "/Volumes/data/Users/MCampbell/" + mouse + "/VR/" + session + "_licks.txt";

	}

	void ConfigurePins( )
	{
		arduino.pinMode (12, PinMode.OUTPUT);
		arduino.pinMode(pin, PinMode.ANALOG);
		arduino.reportAnalog(pin, 1);
	}

	void Update () 
	{

		// check for licks every frame
		pinValue = arduino.analogRead(pin);

		if (pinValue > 500 & lickFlag == 1) 
		{
			lickFlag = 0;
		}
		if (pinValue<500 & lickFlag==0)
		{
			numLicks += 1;
			lickFlag = 1;
			// Debug.Log("Licks: " + numLicks);
			if (saveData)
			{
				var sw = new StreamWriter (lickFile, true);
				sw.Write (transform.position.z + "\t" + Time.realtimeSinceStartup + "\n");
				sw.Close ();
			}

			if (punish & transform.position.z > 10 & transform.position.z < pos1)
			{
				transform.position = punishPosition;
				numMisses = numMisses+1;
				Debug.Log ("Punish: " + numMisses);
			}
			else if (transform.position.z > pos2)
			{
				StartCoroutine (Reward ());
				transform.position = initialPosition;
				numHits = numHits+1;
				Debug.Log ("Hits: " + numHits);
			}
		}
	}

	IEnumerator Reward ()
	{
		arduino.digitalWrite (12, Arduino.HIGH);
		yield return new WaitForSeconds (0.2f);
		arduino.digitalWrite (12, Arduino.LOW);
		if (numHits == numHitsTotal) {
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

	void OnApplicationQuit()
	{
		if (saveData) {
			File.Copy (lickFile, serverLickFile);
		}
	}

}
