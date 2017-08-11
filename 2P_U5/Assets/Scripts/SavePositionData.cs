using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class SavePositionData : MonoBehaviour {

	private SessionParams_2AFC paramsScript;
	private string mouse;
	private string session;
	private bool saveData;
	private string positionFile;
	private string serverPositionFile;
	private string localDirectory;
	private string serverDirectory;

    private static bool created = false;
    public void Awake()
    {
        if (!created)
        {
            // this is the first instance - make it persist
            DontDestroyOnLoad(this);
            created = true;
        }
        else
        {
            // this must be a duplicate from a scene reload - DESTROY!
            Destroy(this);
        }
    }

    void Start()
	{
		GameObject player = GameObject.Find ("Player");
		paramsScript = player.GetComponent<SessionParams_2AFC> ();
		mouse = paramsScript.mouse;
		session = paramsScript.session;
		saveData = paramsScript.saveData;
		localDirectory = paramsScript.localDirectory;
		serverDirectory = paramsScript.serverDirectory;
		positionFile = localDirectory + "/" + mouse + "/" + session + "_position.txt";
		serverPositionFile = serverDirectory + "/" + mouse + "/" + session + "_position.txt";
	}

	void Update () 
	{
		// write position data to file every frame
		if (saveData)
		{
			var sw = new StreamWriter (positionFile, true);
			sw.Write (transform.position.z + "\t" + Time.realtimeSinceStartup + "\r\n");
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
