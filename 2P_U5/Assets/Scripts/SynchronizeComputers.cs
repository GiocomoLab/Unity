using UnityEngine;
using System.Collections;
using Uniduino;
using System;
using System.IO;

public class SynchronizeComputers : MonoBehaviour {

	public Arduino arduino;
	private int startPin = 11;
	private int syncPin = 10;
	private int startPinValue;
	private int startPinValue_last;
	private int syncPinValue;
	private int syncPinValue_last;
	public bool recordingStarted = false;

	// for saving data
	private string localDirectory;
	private string serverDirectory;
	private SessionParams paramsScript;
	private string mouse;
	private string session;
	private bool saveData;
	private string timesyncFile;
	private string serverTimesyncFile;

	void Start () {

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
		localDirectory = paramsScript.localDirectory;
		serverDirectory = paramsScript.serverDirectory;
		timesyncFile = localDirectory + "/" + mouse + "/" + session + "_time_sync.txt";
		serverTimesyncFile = serverDirectory + "/" + mouse + "/VR/" + session + "_time_sync.txt";
	}

	void ConfigurePins () {
		arduino.pinMode (startPin, PinMode.INPUT);
		arduino.pinMode (syncPin, PinMode.INPUT);
		arduino.reportDigital ((byte)(startPin/8),1);
		arduino.reportDigital ((byte)(syncPin/8),1);
		Debug.Log ("Pins configured (synchronize computers)");
	}

	void Update () {
		if (recordingStarted == false)
		{
			startPinValue = arduino.digitalRead (startPin);
			if (startPinValue == Arduino.HIGH & startPinValue_last == Arduino.LOW)
			{
				if (saveData)
				{
					var sw = new StreamWriter (timesyncFile, true);
					sw.Write (Time.realtimeSinceStartup + "\n");
					sw.Close ();
				}
				// start game
				recordingStarted = true;
				Debug.Log ("Session started");
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
