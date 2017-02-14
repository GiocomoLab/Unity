using UnityEngine;
using System.Collections;
using Uniduino;
using System;
using System.IO;

public class SynchronizeComputers : MonoBehaviour {
	
	public Arduino arduino;
	private int pin = 10;
	private int pinValue;
	private int pinValue_last;
	//private bool recordingStarted = false;
	//private Camera mainCam;
	
	void Start () {
		// set screen to black
		//mainCam = Camera.main;
		//mainCam.enabled = false;

		// initialize arduino
		arduino = Arduino.global;
		arduino.Setup(ConfigurePins);
		pinValue = arduino.digitalRead (pin);
		pinValue_last = pinValue;
	}

	void ConfigurePins () {
		arduino.pinMode (pin, PinMode.INPUT);
		arduino.reportDigital ((byte)(pin/8),1);
	}

	void Update () {
		pinValue = arduino.digitalRead(pin);
		Debug.Log (pinValue);
		if (pinValue == Arduino.HIGH & pinValue_last == Arduino.LOW)
		{
			var sw = new StreamWriter ("/Users/malcolmc/Desktop/time_sync.txt", true);
			sw.Write (Time.realtimeSinceStartup + "\n");
			sw.Close ();
		}
		pinValue_last = pinValue;
	}
}
