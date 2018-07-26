using UnityEngine;
using System.Collections;
using System;
using System.IO;
using Uniduino;

public class DetectLicks : MonoBehaviour {

	public Arduino arduino;

	public int pin = 0;
	public int pinValue;
	private int numLicks = 0;
	private int lickFlag = 0;

	// for saving data
	private string localDirectory;
	private string serverDirectory;
	private SessionParams paramsScript;
	private string mouse;
	private string session;
	private bool saveData;
	private string lickFile;
	private string serverLickFile;

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
		localDirectory = paramsScript.localDirectory;
		serverDirectory = paramsScript.serverDirectory;
		lickFile = localDirectory + "/" + mouse + "/" + session + "_licks.txt";
		serverLickFile = serverDirectory + "/" + mouse + "/VR/" + session + "_licks.txt";
	}

	void ConfigurePins( )
	{
		arduino.pinMode(pin, PinMode.ANALOG);
		arduino.reportAnalog(pin, 1);
		Debug.Log ("Pins configured (detect licks)");
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
			if (saveData)
			{
				var sw = new StreamWriter (lickFile, true);
				sw.Write (transform.position.z + "\t" + Time.realtimeSinceStartup + "\n");
				sw.Close ();
			}
		}
	}

	void OnApplicationQuit()
	{
		if (saveData) {
			File.Copy (lickFile, serverLickFile);
		}
	}

}
