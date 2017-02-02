using UnityEngine;
using System.Collections;
using Uniduino;
using System;
using System.IO;

public class SynchronizeComputers_track5 : MonoBehaviour {
	
	public Arduino arduino;
	private int startPin = 11;
	private int syncPin = 10;
	private int startPinValue;
	private int startPinValue_last;
	private int syncPinValue;
	private int syncPinValue_last;
	public bool recordingStarted = false;
	private Camera mainCam;

	// for saving data
	private SessionParams paramsScript;
	private string mouse;
	private string session;
	private bool saveData;
	private string timesyncFile;
	private string serverTimesyncFile;
	
	void Start () {
		// set screen to black and pause game
		mainCam = Camera.main;
		// mainCam.enabled = false;
		//Time.timeScale = 0;

		// initialize arduino
		arduino = Arduino.global;
		arduino.Setup(ConfigurePins);
		startPinValue = arduino.digitalRead (startPin);
		startPinValue_last = startPinValue;
		syncPinValue = arduino.digitalRead (syncPin);
		syncPinValue_last = syncPinValue;

		// for saving data
		GameObject player = GameObject.Find ("Player");
		paramsScript = player.GetComponent<SessionParams> ();
		mouse = paramsScript.mouse;
		session = paramsScript.session;
		saveData = paramsScript.saveData;
		timesyncFile = "/Users/malcolmc/Desktop/" + mouse + "/" + session + "_time_sync.txt";
		serverTimesyncFile = "/Volumes/data/Users/MCampbell/" + mouse + "/VR/" + session + "_time_sync.txt";
	}

	void ConfigurePins () {
		arduino.pinMode (startPin, PinMode.INPUT);
		arduino.pinMode (syncPin, PinMode.INPUT);
		arduino.reportDigital ((byte)(startPin/8),1);
		arduino.reportDigital ((byte)(syncPin/8),1);
	}

	void Update () {
		if (recordingStarted == false)
		{
			startPinValue = arduino.digitalRead (startPin);
			Debug.Log (startPinValue);
			if (startPinValue == Arduino.HIGH & startPinValue_last == Arduino.LOW)
			{
				if (saveData)
				{
					var sw = new StreamWriter (timesyncFile, true);
					sw.Write (Time.realtimeSinceStartup + "\n");
					sw.Close ();
				}

				// start game
				mainCam.enabled = true;
				//Time.timeScale = 1;
				recordingStarted = true;
			}
			startPinValue_last = startPinValue;
		}
		else {
			syncPinValue = arduino.digitalRead(syncPin);
			if (syncPinValue == Arduino.HIGH & syncPinValue_last == Arduino.LOW)
			{
				if (saveData)
				{
					var sw = new StreamWriter (timesyncFile, true);
					sw.Write (Time.realtimeSinceStartup + "\n");
					sw.Close ();
				}
			}
			syncPinValue_last = syncPinValue;
		}
	}

	void OnApplicationQuit()
	{
		if (saveData) {
				File.Copy (timesyncFile, serverTimesyncFile);
		}
	}

}
