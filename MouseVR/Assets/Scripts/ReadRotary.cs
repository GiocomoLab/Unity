using UnityEngine;
using System.Collections;
using System;
using System.IO;
using Uniduino;

public class ReadRotary : MonoBehaviour {

	public Arduino arduino;
	public int pinA = 2;
	public int pinB = 3;
	private int pinA_value = 1;
	private int last_pinA_value = 1;
	private int pinB_value = 1;
	private int pulses = 0;
	private int pulses_last = 0;
	public float speed;
	
	void Start()
	{
		arduino = Arduino.global;
		arduino.Setup(ConfigurePins);
		ReadEncoder ();
	}

	void ConfigurePins()
	{
		arduino.pinMode (pinA, PinMode.INPUT);
		arduino.pinMode (pinB, PinMode.INPUT);
		arduino.reportDigital ((byte)(pinA/8), 1);
		arduino.reportDigital ((byte)(pinB/8), 1);
	}
	
	void Update() 
	{
		if (Input.GetKeyDown (KeyCode.UpArrow)) 
		{
			speed = speed*2;
		}
		if (Input.GetKeyDown (KeyCode.DownArrow)) 
		{
			speed = speed/2;	
		}
		float delta_z = (pulses - pulses_last) * speed;
		Vector3 movement = new Vector3 (0.0f, 0.0f, delta_z);
		transform.position = transform.position-movement;
		pulses_last = pulses;
	}

	private void ReadEncoder()
	{
		arduino.DigitalDataReceived += delegate(int portNumber, int portData) 					
		{
			pinA_value = arduino.digitalRead (pinA);
			pinB_value = arduino.digitalRead (pinB);
			if (pinA_value == 0 & last_pinA_value == 1) {
				if (pinB_value == 1) { pulses ++; }
				else { pulses --; }
			}
			else if (pinA_value == 1 & last_pinA_value == 0) {
				if (pinB_value == 0) { pulses ++; }
				else { pulses --; }
			}
			last_pinA_value = pinA_value;						
		};
	}

}
