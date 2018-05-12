using UnityEngine;
using System.Collections;
using System;
using System.IO;
using Uniduino;

public class DL_MovingWall_LED : MonoBehaviour
{

    public Arduino arduino;
    public int pin = 0;
    private int pinValue;

    private GameObject player;
    private SP_MovingWall sp;
    private PC_MovingWall pc;
    public int r = 0;
    public bool rflag = false;
    private float lastlick;

    void Awake()
    {
        player = GameObject.Find("Player");
        sp = player.GetComponent<SP_MovingWall>();
        pc = player.GetComponent<PC_MovingWall>();
    }

    // Use this for initialization
    void Start()
    {
        arduino = Arduino.global;
        arduino.Setup(ConfigurePins);

    }

    void ConfigurePins()
    {
        arduino.pinMode(pin, PinMode.ANALOG);
        arduino.reportAnalog(pin, 1);
        Debug.Log("Pins configured (detect licks)");
    }

    // Update is called once per frame
    void Update()
    {

        // check for licks every frame
        pinValue = arduino.analogRead(pin);
        Debug.Log(pinValue);
        if (pinValue < 500)
        {

           
                var sw = new StreamWriter(sp.lickFile, true);
                sw.Write(transform.position.z + "\t" + Time.realtimeSinceStartup + "\r\n");
                sw.Close();
            

            if (pc.cmd ==1 & !rflag)
            {
                r = 1;
                pc.cmd = 3;
            } 
        } 


    }
}
