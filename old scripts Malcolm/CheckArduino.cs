using UnityEngine;
using System.Collections;
using Uniduino;
using System;
using System.IO;

public class CheckArduino : MonoBehaviour {

	public Arduino arduino;
	
	void Start () {
		// initialize Arduino
		arduino = Arduino.global;
		arduino.Setup(ConfigurePins);
	}

	void ConfigurePins () {
		arduino.pinMode (10, PinMode.INPUT);
	}

	// every frame, check for Arduino trigger input
	// if found, write time to file
	void Update () {
		if(arduino.digitalRead(10) == Arduino.HIGH)
		{
			var sw = new StreamWriter ("/Users/malcolmc/Desktop/arduinoTrigger.txt", true);
			sw.Write (Time.realtimeSinceStartup + "\n");
			sw.Close ();
		}
	}
}
