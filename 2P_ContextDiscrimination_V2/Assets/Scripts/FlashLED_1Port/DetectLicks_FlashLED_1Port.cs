using UnityEngine;
using System.Collections;
using System;
using System.IO;
using Uniduino;

public class DetectLicks_FlashLED_1Port : MonoBehaviour
{

    public Arduino arduino;
    public int pin = 0;
    private int pinValue;

    private GameObject player;
    private SP_FlashLED_1Port sp;
    private PC_FlashLED_1Port pc;
    public int r = 0;
    public int rflag = 0;
    private float lastlick;

    void Awake()
    {
        player = GameObject.Find("Player");
        sp = player.GetComponent<SP_FlashLED_1Port>();
        pc = player.GetComponent<PC_FlashLED_1Port>();
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
        
        float currTime = Time.realtimeSinceStartup;
        if (pinValue < 500)
        {
            
            if (sp.saveData)
            {
                var sw = new StreamWriter(sp.lickFile, true);
                sw.Write(transform.position.z + "\t" + Time.realtimeSinceStartup + "\r\n");
                sw.Close();
            }

            if (pc.cmd>0)
            {
                r = 1;
                lastlick = currTime;
                pc.cmd = 0;
            }
        }


    }
}
