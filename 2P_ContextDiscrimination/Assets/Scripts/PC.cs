using UnityEngine;
using System;
using System.Collections;
using Uniduino;
using System.IO;
using System.IO.Ports;
using System.Threading;




public class PC : MonoBehaviour
{

    public float rDur = 1.5f; // timeout duration between available rewards
    public Arduino arduino;

    private static int numRewards = 0;
    private int numRewards_manual = 0;
    private int rewardFlag = 0;

    // for saving data
    private string localDirectory;
    private string serverDirectory;
    private SP sp;
    private string mouse;
    private string session;
    private string rewardFile;
    private string serverRewardFile;
    private bool saveData;

    private GameObject player;
    private DetectLicks_2Port dl;
    private ReadRotary rotary;
    private LinAct actuator;
    private NameCheck nc;

    private bool reward_dir;


    public int numTraversals = 0;
    public int numTrialsTotal;
    public float mrd = 50.0f; // minimum reward distance
    public float ard = 150.0f; // additional reward distance
    public bool punish = false;

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

        // initialize arduino
        arduino = Arduino.global;
        arduino.Setup(ConfigurePins);
    }

    void Start()
    {
        // get game objects
        GameObject player = GameObject.Find("Player");
        sp = player.GetComponent<SP>();
        rotary = player.GetComponent<ReadRotary>();
        dl = player.GetComponent<DetectLicks_2Port>();
        actuator = player.GetComponent<LinAct>();


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

        if (!flashFlag & sp.sceneName.Equals("Flash_LED_Cue",StringComparison.Ordinal))
        {
            StartCoroutine(FlashLED());
            flashFlag = true;
        }


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
                var sw = new StreamWriter(sp.rewardFile, true);
                sw.Write(rotary.delta_z + "\t" + Time.realtimeSinceStartup + "\t" + -1.0f + "\t" + 2f + "\r\n");
                sw.Close();
            }
        }

        if (Input.GetKeyDown(KeyCode.Backspace)) // punish
        {
            StartCoroutine(Punish());
        }


    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.tag);
        if (other.tag == "Start")
        {
            sound.Play();
            StartCoroutine(LightsOn());
        }

        else if (other.tag == "Reward")
        {
            int side = Convert.ToInt32(UnityEngine.Random.value <= sp.morph) + 1;
            var sw = new StreamWriter(sp.rewardFile, true);
            sw.Write(transform.position.z + "\t" + Time.realtimeSinceStartup + "\t" + sp.morph + "\t" + side + "\r\n");
            sw.Close();

            StartCoroutine(RewardSequence(side));
            numRewards += 1;


            movement = new Vector3(0.0f, 0.0f, mrd + UnityEngine.Random.value * ard);
            other.gameObject.transform.position = other.gameObject.transform.position + movement;


        }
        else if (other.tag == "Teleport")
        {
            numTraversals += 1;
            StartCoroutine(LightsOff());
            sound.Stop();
            transform.position = initialPosition;

            rewards[0].transform.position = new Vector3(0.0f, 0.0f, 50.0f);

        }

    }

    IEnumerator RewardSequence(int side)
    {   // water reward
        arduino.analogWrite(3,20); // turn LED on
        actuator.actuatorPort.Write("1"); // move forward
        if (side == 1)
        { cmd = 1; }
        else if (side == 2)
        { cmd = 2; };
        yield return new WaitForSeconds(rDur);
        arduino.analogWrite(3,0); // turn LED off
        actuator.actuatorPort.Write("2"); // move port back
        yield return new WaitForSeconds(.05f);
        cmd = 3;
        yield return new WaitForSeconds(.5f);
        cmd = 0;

    }


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
