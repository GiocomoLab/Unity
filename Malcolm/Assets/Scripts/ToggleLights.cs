using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class ToggleLights : MonoBehaviour 
{
	
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
		//mainCam.enabled = false;
		initialBackgroundColor = mainCam.backgroundColor;
		initialAmbientLight = RenderSettings.ambientLight;
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
