using UnityEngine;
using System.Collections;
using System.IO;

public class SessionParams_infiniteTrack : MonoBehaviour {

	//public string sessionName = System.DateTime.Now.ToString ("MM/DD/YYYY");
	public bool saveData = false;
	public string mouse;
	public string session;
	public int recordingLength = 600;
	private float speedVR;
	private ReadRotary_infiniteTrack rotaryScript;
	private string paramsFile;
	private string serverParamsFile;
	
	void Start () 
	{

		GameObject player = GameObject.Find ("Player");
		rotaryScript = player.GetComponent<ReadRotary_infiniteTrack> ();
		speedVR = rotaryScript.speed;
		paramsFile = "/Users/malcolmc/Desktop/" + mouse + "/" + session + "_params.txt";
		serverParamsFile = "/Volumes/data/Users/MCampbell/" + mouse + "/VR/" + session + "_params.txt";

		if (saveData) 
		{
			var sw = new StreamWriter (paramsFile, true);
			sw.WriteLine(recordingLength);
			sw.WriteLine(speedVR);
			sw.WriteLine("finite7");
			sw.WriteLine ("normal");
			sw.Close ();
		}
	}

	void OnApplicationQuit() 
	{
		if (saveData) 
		{
			File.Copy (paramsFile, serverParamsFile);
		}
	}
}
