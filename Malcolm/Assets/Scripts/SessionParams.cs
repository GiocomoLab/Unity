using UnityEngine;
using System.Collections;
using System.IO;

public class SessionParams : MonoBehaviour {

	//public string sessionName = System.DateTime.Now.ToString ("MM/DD/YYYY");
	public bool saveData = false;
	public string mouse;
	public string session;
	public int recordingLength = 600;
	private float speedVR;
	private PlayerController_track11 playerScript;
	private ReadRotary2 rotaryScript;
	private bool manipSession;
	private bool interleavedTrials;
	private string paramsFile;
	private string serverParamsFile;
	
	void Start () 
	{

		GameObject player = GameObject.Find ("Player");
		playerScript = player.GetComponent<PlayerController_track11> ();
		rotaryScript = player.GetComponent<ReadRotary2> ();
		speedVR = rotaryScript.speed;
		manipSession = playerScript.manipSession;
		interleavedTrials = false; // playerScript.interleavedTrials;
		paramsFile = "/Users/malcolmc/Desktop/" + mouse + "/" + session + "_params.txt";
		serverParamsFile = "/Volumes/data/Users/MCampbell/" + mouse + "/VR/" + session + "_params.txt";

		if (saveData) 
		{
			var sw = new StreamWriter (paramsFile, true);
			sw.WriteLine(recordingLength);
			sw.WriteLine(speedVR);
			sw.WriteLine("finite7");
			if (manipSession)
			{
				if (interleavedTrials)
				{
					sw.WriteLine("interleaved");
				} else {
					sw.WriteLine ("block");
				}
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
