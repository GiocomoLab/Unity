using UnityEngine;
using System.Collections;
using Uniduino;
using System;
using System.IO;

public class WaitForKeypress : MonoBehaviour 
{

	public Arduino arduino;
	private Camera mainCam;
	public Light light1;
	public Light light2;
	public Light light3;
	public Light rewardLight;
	private Color initialBackgroundColor;
	private Color initialAmbientLight;
	
	void Start () 
	{
		// set screen to black
		mainCam = Camera.main;
		mainCam.enabled = false;
		initialBackgroundColor = mainCam.backgroundColor;
		initialAmbientLight = RenderSettings.ambientLight;

		// initialize arduino
		arduino = Arduino.global;
		arduino.Setup(ConfigurePins);

		// wait for enter key to be pressed to start game
		StartCoroutine ("Wait");
	
	}

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.F1)) 
		{
			StartCoroutine ("ScreenOff");
		}

		if (Input.GetKeyDown (KeyCode.F2)) 
		{
			StartCoroutine ("ScreenOn");
		}
	}


	void ConfigurePins () {
		arduino.pinMode (11, PinMode.OUTPUT);
	}

	IEnumerator Wait () 
	{
		// pause game
		//Time.timeScale = 0;

		// wait for keypress
		while (!Input.GetKeyDown(KeyCode.Return)) {
			yield return null;
		}

		// open time sync file
		var sw = new StreamWriter ("/Users/malcolmc/Desktop/time_sync.txt", true);

		// start recording
		arduino.digitalWrite (11, Arduino.HIGH);

		// write time to file
		sw.Write ("Start\t" + Time.realtimeSinceStartup + "\n");
		
		// turn on camera
		mainCam.enabled = true;

		// unpause game
		//Time.timeScale = 1;

		// leave trigger on for 500 ms
		yield return new WaitForSeconds(0.5f);

		// turn off trigger
		arduino.digitalWrite (11, Arduino.LOW);

		// close time sync file
		sw.Close ();
	}

	IEnumerator ScreenOff()
	{
		mainCam.clearFlags = CameraClearFlags.SolidColor;
		mainCam.backgroundColor = Color.black;
		light1.enabled = false;
		light2.enabled = false;
		light3.enabled = false;
		rewardLight.enabled = false;
		RenderSettings.ambientLight = Color.black;
		yield return null;
	}

	IEnumerator ScreenOn()
	{
		mainCam.clearFlags = CameraClearFlags.Skybox;
		mainCam.backgroundColor = initialBackgroundColor;
		light1.enabled = true;
		light2.enabled = true;
		light3.enabled = true;
		rewardLight.enabled = true;
		RenderSettings.ambientLight = initialAmbientLight;
		yield return null;
	}

}
