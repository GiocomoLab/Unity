using UnityEngine;
using System;
using System.Collections;
using Uniduino;
using System.IO;
using System.IO.Ports;
using System.Threading;

public class PC_FlashLED_1Port : MonoBehaviour
{

    public float r_timeout = 5.0f; // timeout duration between available rewards
    public Arduino arduino;

    private static int numRewards = 0;
    private int numRewards_manual = 0;
    private int rewardFlag = 0;

    // for saving data
    private SP_FlashLED_1Port sp;
    private GameObject player;
    private DetectLicks_FlashLED_1Port dl;

    public bool pcntrl_pins = false;
    private bool reward_dir;
    private bool flashFlag;

    private static bool created = false;
    private int r;
    private int r_last = 0;

    public int cmd = 0;

    public void Awake()
    {
        // get game objects 
        GameObject player = GameObject.Find("Player");
        sp = player.GetComponent<SP_FlashLED_1Port>();
        dl = player.GetComponent<DetectLicks_FlashLED_1Port>();
    }

    void Start()
    {
        // initialize arduino
        arduino = Arduino.global;
        arduino.Setup(ConfigurePins);
    }

    void ConfigurePins()
    {   // lickports
        arduino.pinMode(12, PinMode.OUTPUT); // single lickport solenoid
        arduino.pinMode(3, PinMode.PWM); // LED

        Debug.Log("Pins configured (player controller)");
        pcntrl_pins = true;
    }

    void Update()
    {
        if (!flashFlag) { StartCoroutine(FlashLED()); flashFlag = true; };

        
        if (dl.r > 0) { StartCoroutine(DeliverReward(dl.r)); dl.r = 0; }; // deliver appropriate reward 

        // manual rewards and punishments
        if (Input.GetKeyDown(KeyCode.Q) | Input.GetMouseButtonDown(0)) // reward left
        {
            numRewards_manual += 1;
            Debug.Log(numRewards_manual);
            StartCoroutine(DeliverReward(11));

            if (sp.saveData)
            {
                var sw = new StreamWriter(sp.manRewardFile, true);
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

    IEnumerator FlashLED()
    {
        while (true)
        {

            arduino.analogWrite(3, 20); // turn LED on
            cmd = 1;// tell DetectLicks to report first lick
            yield return new WaitForSeconds(sp.rDur);
            cmd = 0;
            arduino.analogWrite(3, 0);
            float timeout = 5.0f * UnityEngine.Random.value + 5.0f;
            yield return new WaitForSeconds(timeout);

        }
    }

    IEnumerator DeliverReward(int r)
    { // deliver 
        if (r == 1) // reward
        {
            arduino.digitalWrite(12, Arduino.HIGH);
            yield return new WaitForSeconds(0.1f);
            arduino.digitalWrite(12, Arduino.LOW);
            numRewards += 1;
            Debug.Log("Reward!");
            Debug.Log(numRewards);
        }

        else { yield return null; };

    }

}
