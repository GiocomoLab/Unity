using UnityEngine;
using System.Collections;
using System.IO;

public class SessionParams : MonoBehaviour {

	public bool saveData = false;
	public string mouse;
	public string session;
	private float speedVR;
	private ReadRotary rotaryScript;

	// for saving data
	public string localDirectory = "/Users/malcolmc/Desktop";
	public string serverDirectory = "/Volumes/data/Users/MCampbell";
	private string paramsFile;
	private string serverParamsFile;

	// more session params
	public int numTrialsTotal = 100;
	public bool manipSession = false;
	public bool cueRemovalSession = false;
	public int numTrialsA = 15;
	public int numTrialsB = 10;
	public float speedGain = 1.0f;

	
	void Start () 
	{

		GameObject player = GameObject.Find ("Player");
		rotaryScript = player.GetComponent<ReadRotary> ();
		speedVR = rotaryScript.speed;
		paramsFile = localDirectory + "/" + mouse + "/" + session + "_params.txt";
		serverParamsFile = serverDirectory + "/" + mouse + "/VR/" + session + "_params.txt";

		if (saveData) 
		{
			var sw = new StreamWriter (paramsFile, true);
			sw.WriteLine("3600");
			sw.WriteLine(speedVR);
			sw.WriteLine("finite7");
			if (manipSession)
			{
				sw.WriteLine ("block");
			} else {
				sw.WriteLine ("normal");
			}
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
