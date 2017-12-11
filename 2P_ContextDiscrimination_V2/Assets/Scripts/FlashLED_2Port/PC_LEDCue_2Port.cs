using UnityEngine;
using System;
using System.Collections;
using Uniduino;
using System.IO;
using System.IO.Ports;
using System.Threading;

public class PC_LEDCue_2Port : MonoBehaviour
{

    public float r_timeout = 5.0f; // timeout duration between available rewards
    public Arduino arduino;

    private static int numRewards = 0;
    private int numRewards_manual = 0;
    private int rewardFlag = 0;

    // for saving data
    private SP_LEDCue_2Port sp;
    private GameObject player;
    private DL_LEDCue_2Port dl;

    public bool pcntrl_pins = false;
    private bool reward_dir;
    private bool flashFlag;

    private static bool created = false;
    private int r;
    private int r_last = 0;

    public int cmd = 0;

    public ArrayList LickHistory;

    public void Awake()
    {
        // get game objects 
        GameObject player = GameObject.Find("Player");
        sp = player.GetComponent<SP_LEDCue_2Port>();
        dl = player.GetComponent<DL_LEDCue_2Port>();

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
        cmd = 4;
    }

    void ConfigurePins()
    {   // lickports
        arduino.pinMode(11, PinMode.OUTPUT); // R
        arduino.pinMode(10, PinMode.OUTPUT); // L        
        arduino.pinMode(3, PinMode.PWM); // LED

        Debug.Log("Pins configured (player controller)");
        pcntrl_pins = true;
    }

    void Update()
    {
        
        arduino.analogWrite(3, 20);
        if (dl.r > 0) { StartCoroutine(DeliverReward(dl.r)); dl.r = 0; }; // deliver appropriate reward 


        
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

    
    

    IEnumerator DeliverReward(int r)
    { // deliver 
        if (r == 1) // reward left
        {
            arduino.digitalWrite(11, Arduino.HIGH);
            yield return new WaitForSeconds(0.05f);
            arduino.digitalWrite(11, Arduino.LOW);
            sp.numRewards += 1;
            Debug.Log(sp.numRewards);
            LickHistory.Add(0f);

        }
        else if (r == 2) // reward right
        {
            arduino.digitalWrite(10, Arduino.HIGH);
            yield return new WaitForSeconds(0.05f);
            arduino.digitalWrite(10, Arduino.LOW);
            sp.numRewards += 1;
            Debug.Log(sp.numRewards);
            LickHistory.Add(1f);
        }
        
        else if (r == 11)
        {
            arduino.digitalWrite(11, Arduino.HIGH);
            yield return new WaitForSeconds(0.05f);
            arduino.digitalWrite(11, Arduino.LOW);
            sp.numRewards_manual += 1;

        }
        else if (r == 12)
        {
            arduino.digitalWrite(10, Arduino.HIGH);
            yield return new WaitForSeconds(0.05f);
            arduino.digitalWrite(10, Arduino.LOW);
            sp.numRewards_manual += 1;

        }
        else if (r == 21) // big reward
        {
            arduino.digitalWrite(10, Arduino.HIGH);
            yield return new WaitForSeconds(0.5f);
            arduino.digitalWrite(10, Arduino.LOW);
            sp.numRewards_manual += 1;

        }
        else if (r == 22) // big reward
        {
            arduino.digitalWrite(11, Arduino.HIGH);
            yield return new WaitForSeconds(0.5f);
            arduino.digitalWrite(11, Arduino.LOW);
            sp.numRewards_manual += 1;

        }
        else { yield return null; };

        if (LickHistory.Count > 10)
        {
            LickHistory.RemoveAt(0);
        }
    }

}
