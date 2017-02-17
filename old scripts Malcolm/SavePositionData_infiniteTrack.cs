using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class SavePositionData_infiniteTrack : MonoBehaviour {

	private SessionParams_infiniteTrack paramsScript;
	private string mouse;
	private string session;
	private bool saveData;
	private string positionFile;
	private string serverPositionFile;

	void Start()
	{
		GameObject player = GameObject.Find ("Player");
		paramsScript = player.GetComponent<SessionParams_infiniteTrack> ();
		mouse = paramsScript.mouse;
		session = paramsScript.session;
		saveData = paramsScript.saveData;
		positionFile = "/Users/malcolmc/Desktop/" + mouse + "/" + session + "_position.txt";
		serverPositionFile = "/Volumes/data/Users/MCampbell/" + mouse + "/VR/" + session + "_position.txt";
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
