using UnityEngine;
using System;
using System.Collections;
using Uniduino;
using System.IO;
using System.IO.Ports;
using System.Threading;

public class PC_LEDCue : MonoBehaviour {

    public float rDur = 5.0f; // timeout duration between available rewards
    public Arduino arduino;

    private static int numRewards = 0;
    private int numRewards_manual = 0;
    private int rewardFlag = 0;

    // for saving data
    private SP_LEDCue sp;
    private GameObject player;
    private DetectLicks_1Port_LEDCue dl;
 
    public bool pcntrl_pins = false;
    private bool reward_dir;

    private static bool created = false;
    private int r;
    private int r_last = 0;

    public int cmd = 4;

    public void Awake()
    {
        // get game objects 
        GameObject player = GameObject.Find("Player");
        sp = player.GetComponent<SP_LEDCue>();
        dl = player.GetComponent<DetectLicks_1Port_LEDCue>();
    }

    void Start()
    {
        // initialize arduino
        arduino = Arduino.global;
        arduino.Setup(ConfigurePins);
    }

    void ConfigurePins()
    {   // lickports
        arduino.pinMode(8, PinMode.OUTPUT); // single lickport solenoid
        arduino.pinMode(3, PinMode.PWM); // LED

        Debug.Log("Pins configured (player controller)");
        pcntrl_pins = true;
    }

    void Update()
    {

        arduino.analogWrite(3, 20);
        if (dl.r > 0 & dl.rflag < 1) { StartCoroutine(DeliverReward(dl.r)); dl.rflag = 1; }; // deliver appropriate reward 

        // manual rewards and punishments
        if (Input.GetKeyDown(KeyCode.Q) | Input.GetMouseButtonDown(0)) // reward left
        {
            numRewards_manual += 1;
            Debug.Log(numRewards_manual);
            StartCoroutine(DeliverReward(11));

            if (sp.saveData)
            {
                var sw = new StreamWriter(sp.MRewardFile, true);
                sw.Write(Time.realtimeSinceStartup + "\r\n");
                sw.Close();
            }
        }

    }

    // save manipulation data to server
    void OnApplicationQuit()
    {
        arduino.analogWrite(3, 0);
        
    }


    IEnumerator DeliverReward(int r)
    { // deliver 
        if (r == 1) // reward
        {
            arduino.digitalWrite(10, Arduino.HIGH);
            yield return new WaitForSeconds(0.05f);
            arduino.digitalWrite(10, Arduino.LOW);
            numRewards += 1;
            Debug.Log(numRewards);
        }
        
        else { yield return null; };

    }

}
