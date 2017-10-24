using UnityEngine;
using System.Collections;
using Uniduino;
using System;
using System.IO;

public class SynchronizeComputers : MonoBehaviour
{

    public Arduino arduino;
    private int ttl0 = 0;
    private int ttl1 = 1;
    public bool recordingStarted = true;
    public bool sync_pins = false;

    private PC playerScript;
    private int numTraversals_local = 0;

    // for saving data
    private SP sp;
    
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
        // initialize arduino
        
        // for saving data
        GameObject player = GameObject.Find("Player");
        sp = player.GetComponent<SP>();


        sync_pins = true;
    }

    void Start()
    {

        
        // start game
        Debug.Log("TTLs starting");
        var sw = new StreamWriter(sp.timeSyncFile, true);
        sw.Write(Time.realtimeSinceStartup + "\n");
        sw.Close();
        arduino = Arduino.global;
        arduino.Setup(ConfigurePins);
        //start first trial ttl1
        arduino.digitalWrite(ttl1, Arduino.HIGH);

        




    }

    void ConfigurePins()
    {
        arduino.pinMode(ttl0, PinMode.OUTPUT);
        arduino.pinMode(ttl1, PinMode.OUTPUT);
        Debug.Log("Pins configured (synchronize computers)");
    }

    void Update()
    {
        // send a TTL to scanbox every Unity frame
        arduino.digitalWrite(ttl0, Arduino.LOW);
        arduino.digitalWrite(ttl0, Arduino.HIGH);
        // send a TTL on a different pin at the start of every new trial
        if (sp.numTraversals > numTraversals_local)
        {
            numTraversals_local = sp.numTraversals;
            arduino.digitalWrite(ttl1, Arduino.LOW);
            arduino.digitalWrite(ttl1, Arduino.HIGH);

        }



    }

    
}
