﻿using UnityEngine;
using System.Collections;
using System;
using System.IO;
using Uniduino;

public class DetectLicks_1Port_LEDCue : MonoBehaviour {

    public Arduino arduino;
    public int pin = 0;
    private int pinValue;

    private GameObject player;
    private SP_LEDCue sp;
    public int r = 0;
    public int rflag = 0;
    private float lastlick;

    void Awake()
    {
        player = GameObject.Find("Player");
        sp = player.GetComponent<SP_LEDCue>();
    }

    // Use this for initialization
    void Start () {
        arduino = Arduino.global;
        arduino.Setup(ConfigurePins);
        lastlick = -5;
    }

    void ConfigurePins()
    {
        arduino.pinMode(pin, PinMode.ANALOG);
        arduino.reportAnalog(pin, 1);
        Debug.Log("Pins configured (detect licks)");
    }
	
	// Update is called once per frame
	void Update () {
        rflag = 0;
        // check for licks every frame
        pinValue = arduino.analogRead(pin);
        float currTime = Time.realtimeSinceStartup;
        if (pinValue < 500 & currTime-lastlick > 5)
        {
            r = 1;
            lastlick = currTime;
            if (sp.saveData)
            {
                var sw = new StreamWriter(sp.lickFile, true);
                sw.Write(transform.position.z + "\t" + Time.realtimeSinceStartup + "\n");
                sw.Close();
            }
        }


    }
}
