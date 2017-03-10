using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.IO.Ports;
using System.Threading;

public class ReadRotary : MonoBehaviour {
	
	public float speed = 1;
	private float realSpeed;
	public string port = "/dev/cu.usbmodem1461";
	private int pulses;
	private SerialPort _serialPort;
	private int delay;
	//private bool manipTrial_local = false;
	//private SessionParams paramsScript;
	//private float speedGain;
	//private SynchronizeComputers syncScript;
	//private bool recordingStarted_local = false;
	private PlayerController playerScript;

	void Start()
	{
		// connect to Arduino uno serial port
		connect (port, 57600, true, 3);
		Debug.Log ("Connected to rotary encoder serial port");

		// set speed
		realSpeed = 0;

		// connect to playerController script
		GameObject player = GameObject.Find ("Player");
		playerScript = player.GetComponent<PlayerController> ();
		//paramsScript = player.GetComponent<SessionParams> ();
		//speedGain = paramsScript.speedGain;

		//GameObject gameControl = GameObject.Find ("Game Control");
		//syncScript = gameControl.GetComponent<SynchronizeComputers> ();
	}

	void Update() 
	{
//		if (syncScript.recordingStarted & !recordingStarted_local) 
//		{
//			realSpeed = speed * 0.0447f; // makes 1 VR unit = 1 cm for 18 inch circumference cylinder
//			recordingStarted_local = true;
//		}
//
//		if (Input.GetKeyDown (KeyCode.UpArrow)) 
//		{
//			speed = speed*2;
//			realSpeed = realSpeed*2;
//		}
//		if (Input.GetKeyDown (KeyCode.DownArrow)) 
//		{
//			speed = speed/2;
//			realSpeed = realSpeed/2;	
//		}

		// read quadrature encoder and move player accordingly
		realSpeed = speed*0.0447f;
		_serialPort.Write("\n");
		try {
		pulses = int.Parse (_serialPort.ReadLine ());
		Debug.Log (pulses);
		float delta_z = -1f * pulses * realSpeed;
		Vector3 movement = new Vector3 (0.0f, 0.0f, delta_z);
		transform.position = transform.position+movement;
		} catch(TimeoutException) {
			Debug.Log ("timeout");
		}
		// change speed if manip trial
//		if (paramsScript.manipSession)
//		{
//			if(playerScript.manipTrial & !manipTrial_local)
//			{
//				realSpeed = realSpeed * speedGain;
//				speed = speed * speedGain;
//				manipTrial_local = playerScript.manipTrial;
//			}
//			else if(!playerScript.manipTrial & manipTrial_local)
//			{
//				realSpeed = realSpeed / speedGain;
//				speed = speed / speedGain;
//				manipTrial_local = playerScript.manipTrial;
//			}
//		}

	}

	private void connect(string serialPortName, Int32 baudRate, bool autoStart, int delay)
	{
		_serialPort = new SerialPort(serialPortName, baudRate);
		//_serialPort = Win32SerialPort.CreateInstance();
		
		_serialPort.DtrEnable = true; // win32 hack to try to get DataReceived event to fire
		_serialPort.RtsEnable = true; 
		_serialPort.PortName = serialPortName;
		_serialPort.BaudRate = baudRate;
		
		_serialPort.DataBits = 8;
		_serialPort.Parity = Parity.None;
		_serialPort.StopBits = StopBits.One;
		_serialPort.ReadTimeout = 100; // since on windows we *cannot* have a separate read thread
		_serialPort.WriteTimeout = 1000;

		
		if (autoStart)
		{
			this.delay = delay;
			this.Open();
		}
	}

	private void Open()
	{
		_serialPort.Open();
		
		if (_serialPort.IsOpen)
		{
			Thread.Sleep(delay);
		}
	}
		
	private void Close()
	{		
		if (_serialPort != null)
			_serialPort.Close();
	}

	private void Disconnect()
	{
		Close ();		
	}
	
	void OnDestroy()
	{				
		Disconnect();
	}

}
