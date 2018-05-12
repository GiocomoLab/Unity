using UnityEngine;
using System.Collections;
using Uniduino;
using System;
using System.IO;

public class SynchronizeComputers_2AFC : MonoBehaviour {

	public Arduino arduino;
    private int ttl0 = 0;
    private int ttl1 = 1;
	public bool recordingStarted = true;
    public bool sync_pins = false;

    private PC_2AFC playerScript;
    private int numTraversals_local = 0;
    //private int numTraversals;

    // for saving data
    private string localDirectory;
	private string serverDirectory;
	private SP_2AFC paramsScript;
	private string mouse;
	private string session;
	private bool saveData;
	private string timesyncFile;
	private string serverTimesyncFile;

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

    void Start () {

        // for saving data
        GameObject player = GameObject.Find("Player");
        paramsScript = player.GetComponent<SP_2AFC>();
        playerScript = player.GetComponent<PC_2AFC>();
        Debug.Log(playerScript.numTraversals);
        
        mouse = paramsScript.mouse;
        session = paramsScript.session;
        saveData = paramsScript.saveData;
        localDirectory = paramsScript.localDirectory;
        serverDirectory = paramsScript.serverDirectory;
        timesyncFile = localDirectory + "/" + mouse + "/" + session + "_time_sync.txt";
        serverTimesyncFile = serverDirectory + "/" + mouse + "/" + session + "_time_sync.txt";

        // initialize arduino
        arduino = Arduino.global;
        //arduino.Connect();
		arduino.Setup(ConfigurePins);
        sync_pins = true;

        // start game
        Debug.Log("TTLs starting");
        var sw = new StreamWriter(timesyncFile, true);
        sw.Write(Time.realtimeSinceStartup + "\n");
        sw.Close();

        //start first trial ttl1
        arduino.digitalWrite(ttl1, Arduino.HIGH);
        




        
	}

	void ConfigurePins () {
        arduino.pinMode (ttl0, PinMode.OUTPUT);
        arduino.pinMode(ttl1, PinMode.OUTPUT);
		Debug.Log ("Pins configured (synchronize computers)");
	}

	void Update () {
        // send a TTL to scanbox every Unity frame
        arduino.digitalWrite(ttl0, Arduino.LOW);
        arduino.digitalWrite(ttl0, Arduino.HIGH);
        // send a TTL on a different pin at the start of every new trial
        if (playerScript.numTraversals > numTraversals_local)
        {
            numTraversals_local = playerScript.numTraversals;
            arduino.digitalWrite(ttl1, Arduino.LOW);
            arduino.digitalWrite(ttl1, Arduino.HIGH);
            
        }


        
	}

	void OnApplicationQuit()
	{
		if (saveData) {
			File.Copy (timesyncFile, serverTimesyncFile);
		}
	}

}
