using UnityEngine;
using System;
using System.Collections;
using Uniduino;
using System.IO;
using System.IO.Ports;
using System.Threading;

public class PC_FlashLED_2Port : MonoBehaviour
{

    public float r_timeout = 5.0f; // timeout duration between available rewards
    public Arduino arduino;

    private static int numRewards = 0;
    private int numRewards_manual = 0;
    private int rewardFlag = 0;

    // for saving data
    private SP_FlashLED_2Port sp;
    private GameObject player;
    private DL_FlashLED_2Port dl;

    public bool pcntrl_pins = false;
    private bool reward_dir;
    private bool flashFlag;


    private int RPort = 11;
    private int LPort = 12;
    private int puff = 10;


    private static bool created = false;
    private int r;
    private int r_last = 0;

    public int cmd = 0;

    public ArrayList LickHistory;

    public void Awake()
    {
        // get game objects 
        GameObject player = GameObject.Find("Player");
        sp = player.GetComponent<SP_FlashLED_2Port>();
        dl = player.GetComponent<DL_FlashLED_2Port>();

        LickHistory = new ArrayList();
        int j = 0;
        while (j < 20)
        {
            LickHistory.Add(0.5f);
            j++;
        }
        Debug.Log(LickHistory[0]);
    }

    void Start()
    {
        // initialize arduino
        arduino = Arduino.global;
        arduino.Setup(ConfigurePins);
    }

    void ConfigurePins()
    {   // lickports
        arduino.pinMode(LPort, PinMode.OUTPUT); // R
        arduino.pinMode(RPort, PinMode.OUTPUT); // L        
        arduino.pinMode(3, PinMode.PWM); // LED

        Debug.Log("Pins configured (player controller)");
        pcntrl_pins = true;
    }

    void Update()
    {
        if (!flashFlag) { StartCoroutine(FlashLED()); flashFlag = true; };


        //arduino.analogWrite(3, 20);
        if (dl.r > 0 & dl.rflag < 1) { StartCoroutine(DeliverReward(dl.r)); dl.rflag = 1; }; // deliver appropriate reward 

        // manual rewards and punishments
        if (Input.GetKeyDown(KeyCode.Q) | Input.GetMouseButtonDown(0)) // reward left
        {
            numRewards_manual += 1;
            Debug.Log(numRewards_manual);
            StartCoroutine(DeliverReward(1));

            if (sp.saveData)
            {
                var sw = new StreamWriter(sp.rewardFile, true);
                sw.Write(Time.realtimeSinceStartup + "\t" + 1f + "\r\n");
                sw.Close();
            }
        }

        if (Input.GetKeyDown(KeyCode.P) | Input.GetMouseButtonDown(1)) // reward right
        {
            numRewards_manual += 1;
            Debug.Log(numRewards_manual);
            StartCoroutine(DeliverReward(2));

            if (sp.saveData)
            {
                var sw = new StreamWriter(sp.rewardFile, true);
                sw.Write(Time.realtimeSinceStartup + "\t" + 2f + "\r\n");
                sw.Close();
            }
        }

        r_last = dl.r;

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
            cmd = 4; // reward first lick
            
            // correct biases in licking behavior 
            float sum = 0;
            foreach (float j in LickHistory)
            {
                sum += j;
            }
            float thresh = sum / (float)LickHistory.Count;
            if (thresh >= .9 )
            {
                StartCoroutine(DeliverReward(21)); // if haven't licked left in a while, deliver a reward to the left
                var sw = new StreamWriter(sp.rewardFile, true);
                sw.Write(Time.realtimeSinceStartup + "\t" + 21f + "\r\n");
                sw.Close();

            } else if (thresh <=.1)
            {
                StartCoroutine(DeliverReward(22)); // if haven't licked right in a while, deliver a reward to the right
                var sw = new StreamWriter(sp.rewardFile, true);
                sw.Write(Time.realtimeSinceStartup + "\t" + 22f + "\r\n");
                sw.Close();
            }
            //

            yield return new WaitForSeconds(sp.rDur);
            arduino.analogWrite(3, 0);
            float timeout = 5.0f * UnityEngine.Random.value + 5.0f;
            cmd = 3;
            yield return new WaitForSeconds(.5f);
            cmd = 0;
            yield return new WaitForSeconds(timeout);

        }
    }

    IEnumerator DeliverReward(int r)
    { // deliver 
        if (r == 1) // reward left
        {
            arduino.digitalWrite(LPort, Arduino.HIGH);
            yield return new WaitForSeconds(0.05f);
            arduino.digitalWrite(LPort, Arduino.LOW);
            sp.numRewards += 1;
            Debug.Log(sp.numRewards);
            LickHistory.Add(0f);

        }
        else if (r == 2) // reward right
        {
            arduino.digitalWrite(RPort, Arduino.HIGH);
            yield return new WaitForSeconds(0.05f);
            arduino.digitalWrite(RPort, Arduino.LOW);
            sp.numRewards += 1;
            Debug.Log(sp.numRewards);
            LickHistory.Add(1f);
        }
        
        else if (r == 11)
        {
            arduino.digitalWrite(LPort, Arduino.HIGH);
            yield return new WaitForSeconds(0.05f);
            arduino.digitalWrite(LPort, Arduino.LOW);
            sp.numRewards_manual += 1;

        }
        else if (r == 12)
        {
            arduino.digitalWrite(RPort, Arduino.HIGH);
            yield return new WaitForSeconds(0.05f);
            arduino.digitalWrite(RPort, Arduino.LOW);
            sp.numRewards_manual += 1;

        }
        else if (r == 21) // big reward
        {
            arduino.digitalWrite(LPort, Arduino.HIGH);
            yield return new WaitForSeconds(0.5f);
            arduino.digitalWrite(LPort, Arduino.LOW);
            sp.numRewards_manual += 1;

        }
        else if (r == 22) // big reward
        {
            arduino.digitalWrite(RPort, Arduino.HIGH);
            yield return new WaitForSeconds(0.5f);
            arduino.digitalWrite(RPort, Arduino.LOW);
            sp.numRewards_manual += 1;

        }
        else { yield return null; };

        if (LickHistory.Count > 10)
        {
            LickHistory.RemoveAt(0);
        }
    }

}
