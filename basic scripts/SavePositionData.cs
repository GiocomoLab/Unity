using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class SavePositionData : MonoBehaviour {

	private SessionParams paramsScript;
	private string mouse;
	private string session;
	private bool saveData;
	private string positionFile;
	private string serverPositionFile;
	private string localDirectory;
	private string serverDirectory;

	void Start()
	{
		GameObject player = GameObject.Find ("Player");
		paramsScript = player.GetComponent<SessionParams> ();
		mouse = paramsScript.mouse;
		session = paramsScript.session;
		saveData = paramsScript.saveData;
		localDirectory = paramsScript.localDirectory;
		serverDirectory = paramsScript.serverDirectory;
		positionFile = localDirectory + "/" + mouse + "/" + session + "_position.txt";
		serverPositionFile = serverDirectory + "/" + mouse + "/VR/" + session + "_position.txt";
	}

	void Update () 
	{
		// write position data to file every frame
		if (saveData)
		{
			var sw = new StreamWriter (positionFile, true);
			sw.Write (transform.position.z + "\t" + Time.realtimeSinceStartup + "\n");
			sw.Close ();
		}
	}

	void OnApplicationQuit()
	{
		if (saveData) 
		{
			File.Copy (positionFile, serverPositionFile);
		}
	}
	
}
