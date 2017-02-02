using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.IO.Ports;
using System.Threading;

public class ReadRotary2 : MonoBehaviour {
	
	public float speed = 1;
	private float realSpeed;
	private float originalSpeed;
	private float originalRealSpeed;
	public string port = "/dev/tty.usbmodem1431";
	private int pulses;
	private SerialPort _serialPort;
	private int delay;
	private bool manipTrial_local = false;
	private PlayerController_track11 playerScript;
	private float speedGain;
	private SynchronizeComputers_track5 syncScript;
	private bool recordingStarted_local = false;

	void Start()
	{
		// connect to Arduino uno serial port
		connect (port, 57600, true, 3);

		// set speed
		realSpeed = 0;

		// connect to playerController script
		GameObject player = GameObject.Find ("Player");
		playerScript = player.GetComponent<PlayerController_track11> ();
		speedGain = playerScript.speedGain;

		GameObject gameControl = GameObject.Find ("Game Control");
		syncScript = gameControl.GetComponent<SynchronizeComputers_track5> ();
	}

	void Update() 
	{
		if (syncScript.recordingStarted & !recordingStarted_local) 
		{
			realSpeed = speed * 0.0614f * (429f/400f);	
			originalRealSpeed = realSpeed;
			originalSpeed = 1;
			recordingStarted_local = true;
		}

		if (Input.GetKeyDown (KeyCode.UpArrow)) 
		{
			speed = speed*2;
			realSpeed = realSpeed*2;
		}
		if (Input.GetKeyDown (KeyCode.DownArrow)) 
		{
			speed = speed/2;
			realSpeed = realSpeed/2;	
		}

		// read quadrature encoder and move player accordingly
		_serialPort.Write("\n");
		pulses = int.Parse (_serialPort.ReadLine ());
		//Debug.Log (pulses);
		float delta_z = pulses * realSpeed;
		Vector3 movement = new Vector3 (0.0f, 0.0f, delta_z);
		transform.position = transform.position+movement;

		// change speed if manip trial
		if (playerScript.manipSession)
		{
			if(playerScript.manipTrial & !manipTrial_local)
			{
				speedGain = playerScript.speedGain;
				realSpeed = realSpeed * speedGain;
				speed = speed * speedGain;
				manipTrial_local = playerScript.manipTrial;
			}
			else if(!playerScript.manipTrial & manipTrial_local)
			{
				realSpeed = realSpeed / speedGain;
				speed = speed / speedGain;
				manipTrial_local = playerScript.manipTrial;
			}
		} // else if (playerScript.testRangeOfGains) {
		//	speedGain = playerScript.speedGain;
		//	realSpeed = originalRealSpeed * speedGain;
		//	speed = speedGain;
			// Debug.Log (speed);
			// manipTrial_local = playerScript.manipTrial;
		// }
	}

	void OnTriggerEnter (Collider other)
	{
//		if (other.tag == "Teleport") {
//			numTraversals += 1;
//			transform.position = initialPosition;
//			Debug.Log ("Traversals: " + numTraversals);
//
//			// DO TRIAL TYPE MANIPULATION
//			if (manipSession) {
//					if (numTraversals == numTrialsA) {
//							speed = speed * speedGain;
//							manipTrial = true;
//											
//							// write time to file
//							var sw = new StreamWriter ("/Users/malcolmc/Desktop/manip_times.txt", true);
//							sw.Write (Time.realtimeSinceStartup + "\n");
//							sw.Close ();
//					} else if (numTraversals == numTrialsA + numTrialsB) {
//							speed = speed / speedGain;
//							manipTrial = false;
//					
//							// write time to file
//							var sw = new StreamWriter ("/Users/malcolmc/Desktop/manip_times.txt", true);
//							sw.Write (Time.realtimeSinceStartup + "\n");
//							sw.Close ();
//					}
//					//var sw = new StreamWriter ("/Users/malcolmc/Desktop/trial_type.txt", true);
//					//sw.Write (Time.realtimeSinceStartup + "\t" + manipTrial + "\n");
//					//sw.Close ();
//					//Debug.Log ("Manip: " + manipTrial);
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
		_serialPort.ReadTimeout = 5; // since on windows we *cannot* have a separate read thread
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

	/// <summary>
	/// Closes the serial port.
	/// </summary>
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
