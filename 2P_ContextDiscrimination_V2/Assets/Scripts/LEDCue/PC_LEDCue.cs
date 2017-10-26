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
    private string localDirectory;
    private string serverDirectory;
    private SP_LEDCue sp;
    private string mouse;
    private string session;
    private string rewardFile;
    private string serverRewardFile;
    private bool saveData;

    private GameObject player;
    private DetectLicks_1Port_LEDCue dl;
    

    private NameCheck nc;

    public bool pcntrl_pins = false;
    private bool reward_dir;

    private static bool created = false;
    private int r;
    private int r_last = 0;

    public int cmd = 4;

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
        // initialize arduino
        arduino = Arduino.global;
        arduino.Setup(ConfigurePins);

        // get game objects 
        GameObject player = GameObject.Find("Player");
        sp = player.GetComponent<SP_LEDCue>();
        dl = player.GetComponent<DetectLicks_1Port_LEDCue>();
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
            StartCoroutine(DeliverReward(1));

            if (paramsScript.saveData)
            {
                var sw = new StreamWriter(rewardFile, true);
                sw.Write(rotary.delta_z + "\t" + Time.realtimeSinceStartup + "\t" + -1.0f + "\t" + 0f + "\r\n");
                sw.Close();
            }
        }

        if (Input.GetKeyDown(KeyCode.P) | Input.GetMouseButtonDown(1)) // reward right
        {
            numRewards_manual += 1;
            Debug.Log(numRewards_manual);
            StartCoroutine(DeliverReward(2));

            if (paramsScript.saveData)
            {
                var sw = new StreamWriter(rewardFile, true);
                sw.Write(rotary.delta_z + "\t" + Time.realtimeSinceStartup + "\t" + -1.0f + "\t" + 2f + "\r\n");
                sw.Close();
            }
        }

        if (Input.GetKeyDown(KeyCode.Backspace)) // punish
        {
            StartCoroutine(Punish());
        }

        r_last = dl.r;


    }

    // save manipulation data to server
    void OnApplicationQuit()
    {
        arduino.analogWrite(3, 0);
        if (paramsScript.saveData)
        {
            File.Copy(rewardFile, serverRewardFile);

        }
    }


    IEnumerator DeliverReward(int r)
    { // deliver 
        if (r == 1) // reward left
        {
            arduino.digitalWrite(10, Arduino.HIGH);
            yield return new WaitForSeconds(0.05f);
            arduino.digitalWrite(10, Arduino.LOW);
            numRewards += 1;
            Debug.Log(numRewards);
        }
        else if (r == 2) // reward right
        {
            arduino.digitalWrite(11, Arduino.HIGH);
            yield return new WaitForSeconds(0.05f);
            arduino.digitalWrite(11, Arduino.LOW);
            numRewards += 1;
            Debug.Log(numRewards);
        }
        else { yield return null; };

    }


    IEnumerator Punish()
    {   // air puff punishment
        arduino.digitalWrite(9, Arduino.HIGH);
        yield return new WaitForSeconds(0.1f);
        arduino.digitalWrite(9, Arduino.LOW);
    }

}
