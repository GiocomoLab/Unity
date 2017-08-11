using UnityEngine;
using System;
using System.Collections;
using Uniduino;
using System.IO;

public class FlashLED_2AFC : MonoBehaviour {

    public Arduino arduino;
    private GameObject player;
    private SessionParams_2AFC paramsScript;
    private PC_2AFC playerController;

    private float i1 = 1; // range is 0 255
    private float freq1 = 0.25f;
    private float i2 = 20;
    private float freq2 =10.0f;

    private int num_traversals_local = 0;
    //private int num_traversals;
    private float morph;

    // Use this for initialization
    void Start () {
        player = GameObject.Find("Player");
        playerController = player.GetComponent<PC_2AFC>();
        paramsScript = player.GetComponent<SessionParams_2AFC>();

        // initialize arduino
        arduino = Arduino.global;
        //arduino.Connect();
        arduino.Setup(ConfigurePins);


        StartCoroutine(flash_loop(paramsScript.morph,num_traversals_local));


    }
	
	// Update is called once per frame
	void Update () {
        if (player.transform.position.z <= 0.0f & morph != paramsScript.morph)
        {
            num_traversals_local= playerController.numTraversals;
            morph = paramsScript.morph; 
            StartCoroutine(flash_loop(morph, num_traversals_local));

        }
	}

    void ConfigurePins()
    {
        // LED 
        arduino.pinMode(3, PinMode.PWM);
    }

    IEnumerator flash_loop(float morph, int run_number)
    {
        float freq = morph * freq1 + (1.0f - morph) * freq2;
        int i = (int)(morph * i1 + (1.0f - morph) * i2);
        while (true ) //player position is positive and num_traversals is the same as local
        {
            if (player.transform.position.z >= 0.0f)
            {
                arduino.analogWrite(3, i);
                yield return new WaitForSeconds(freq / 2.0f);
                arduino.analogWrite(3, 0);
                yield return new WaitForSeconds(freq / 2.0f);
            } else
            {
                arduino.analogWrite(3, 0);
                yield return new WaitForSeconds(.01f);
            }
            if (morph != paramsScript.morph) {
                break;
            }
        }
       
    }


    void OnApplicationQuit()
    {
        arduino.analogWrite(3, 0);
    }
}
