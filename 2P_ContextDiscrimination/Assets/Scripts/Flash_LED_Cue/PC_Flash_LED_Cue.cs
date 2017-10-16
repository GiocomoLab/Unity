using UnityEngine;
using System;
using System.Collections;
using Uniduino;
using System.IO;
using System.IO.Ports;
using System.Threading;




public class PC_Flash_LED_Cue : MonoBehaviour
{

    public float rDur = 1.5f; // timeout duration between available rewards
    public Arduino arduino;

    private static int numRewards = 0;
    private int numRewards_manual = 0;
    private int rewardFlag = 0;

    // for saving data
    private string localDirectory;
    private string serverDirectory;
    private SP_Flash_LED_Cue paramsScript;
    private string mouse;
    private string session;
    private string rewardFile;
    private string serverRewardFile;
    private bool saveData;

    private GameObject player;
    private DetectLicks_2Port_Flash_LED_Cue dl;
    private ReadRotary_Flash_LED_Cue rotary;
    private LinAct_Flash_LED actuator;

    public bool pcntrl_pins = false;
    private bool reward_dir;

    private static bool created = false;
    private int r;
    private int r_last = 0;

    public int cmd = 3;
    private bool flashFlag = false;

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
        paramsScript = player.GetComponent<SP_Flash_LED_Cue>();
        rotary = player.GetComponent<ReadRotary_Flash_LED_Cue>();
        dl = player.GetComponent<DetectLicks_2Port_Flash_LED_Cue>();
        actuator = player.GetComponent<LinAct_Flash_LED>();
        

        // setup file names
        // wait for session params script to check for directory
        string rewardFile_pre = paramsScript.localDirectory + "/" + paramsScript.session + "_Flash_LED_MRewards";
        string serverRewardFile_pre = paramsScript.serverDirectory + "/" + paramsScript.session + "_Flash_LED_MRewards";
        if (paramsScript.saveData)
        {
            while (paramsScript.dirCheck < 1) { }
            // check if file exists
            if (File.Exists(rewardFile_pre + ".txt"))
            {
                rewardFile = rewardFile_pre + "_copy.txt";
                serverRewardFile = serverRewardFile_pre + "_copy.txt";
                var sw = new StreamWriter(rewardFile, true);
                sw.Close();
            }
            else
            {
                rewardFile = rewardFile_pre + ".txt";
                serverRewardFile = serverRewardFile_pre + ".txt";
                var sw = new StreamWriter(rewardFile, true);
                sw.Close();
            }
        }

        

    }

    void ConfigurePins()
    {   // lickports
        arduino.pinMode(11, PinMode.OUTPUT); // R
        arduino.pinMode(10, PinMode.OUTPUT); // L        
        arduino.pinMode(9, PinMode.OUTPUT); // airpuff
        arduino.pinMode(3, PinMode.PWM); // LED


        Debug.Log("Pins configured (player controller)");
        pcntrl_pins = true;
    }

    void Update()
    {   

        if (!flashFlag) {
            StartCoroutine(FlashLED());
            flashFlag = true; }

        //arduino.analogWrite(3, 20);
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
        actuator.actuatorPort.Write("2");
        if (paramsScript.saveData)
        {
            File.Copy(rewardFile, serverRewardFile);

        }
    }

    IEnumerator FlashLED()
    {
        Debug.Log("flash light");
        while (true)
        {
                
            arduino.analogWrite(3, 20); // turn LED on
            actuator.actuatorPort.Write("1"); // move forward
            cmd = 7; // reward first lick
            yield return new WaitForSeconds(rDur);

            arduino.analogWrite(3, 0);
            actuator.actuatorPort.Write("2");  // move backward
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
