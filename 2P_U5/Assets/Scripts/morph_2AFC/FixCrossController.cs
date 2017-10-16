using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using Uniduino;
using System.IO;
using System.IO.Ports;
using System.Threading;

public class FixCrossController : MonoBehaviour {

    public float rDur = 5.0f;
    public Arduino arduino;
    private Vector3 initialPosition;
    private static int numRewards = 0;
    private int numRewards_manual = 0;
    private int rewardFlag = 0;

    // for saving data
    private string localDirectory;
    private string serverDirectory;
    private SessionParams_2AFC paramsScript;
    private string mouse;
    private string session;
    private string rewardFile;
    private string serverRewardFile;
    private bool saveData;

    // lights, for visual landmarks
    public Light light1;
    public Light light2;
    private Color initialBackgroundColor;
    private Color initialAmbientLight;

   
    private GameObject player;
    private GameObject rewardCam;
    private GameObject blackCam;
    private DetectLicks_2Port_FCT dl;
    private ReadRotary_FCT rotary;

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


        // turn off lights
        blackCam = GameObject.Find("Black Camera");
        rewardCam = GameObject.Find("Reward Camera");
        rewardCam.SetActive(true);

        // turn reward camera to proper position
        rewardCam.transform.Rotate(new Vector3(0f, 180f, 0f));

        // get game objects 
        GameObject player = GameObject.Find("Player");
        paramsScript = player.GetComponent<SessionParams_2AFC>();
        rotary = player.GetComponent<ReadRotary_FCT>();
        dl = player.GetComponent<DetectLicks_2Port_FCT>();

        // setup file names
        localDirectory = paramsScript.localDirectory;
        serverDirectory = paramsScript.serverDirectory;
        rewardFile = localDirectory + "/" + paramsScript.mouse + "/" + paramsScript.session + "_" + paramsScript.sessionType + "_MRewards.txt";
        serverRewardFile = serverDirectory + "/" + paramsScript.mouse + "/" + paramsScript.session + "_" + paramsScript.sessionType + "_MRewards.txt";
        var sw = new StreamWriter(rewardFile, true);
        sw.Close();


        if (paramsScript.sessionType.Equals("FFCT",StringComparison.Ordinal)) {
            StartCoroutine(FlashCross());
         };
    }

    void ConfigurePins()
    {   // lickports
        arduino.pinMode(11, PinMode.OUTPUT); // R
        arduino.pinMode(8, PinMode.OUTPUT); // L        

        Debug.Log("Pins configured (player controller)");
        pcntrl_pins = true;
    }

    void Update()
    {

        // make sure rotation angle is 0
        transform.eulerAngles = new Vector3(0.0f, 90.0f, 0.0f);

        if (dl.r > 0 & dl.rflag < 1) { StartCoroutine(DeliverReward(dl.r)); dl.rflag = 1; }; // deliver appropriate reward 

        // manual rewards and punishments
        if (Input.GetKeyDown(KeyCode.Q) | Input.GetMouseButtonDown(0)) // reward left
        {
            numRewards_manual += 1;
            Debug.Log(numRewards_manual);
            StartCoroutine(DeliverReward(1));

            var sw = new StreamWriter(rewardFile, true);
            sw.Write(rotary.delta_z + "\t" + Time.realtimeSinceStartup + "\t" + -1.0f + "\t" + 1f + "\r\n");
            sw.Close();
        }

        if (Input.GetKeyDown(KeyCode.P) | Input.GetMouseButtonDown(1)) // reward right
        {
            numRewards_manual += 1;
            Debug.Log(numRewards_manual);
            StartCoroutine(DeliverReward(2));

            var sw = new StreamWriter(rewardFile, true);
            sw.Write(rotary.delta_z + "\t" + Time.realtimeSinceStartup + "\t" + -1.0f + "\t" + 2f + "\r\n");
            sw.Close();
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
        File.Copy(rewardFile, serverRewardFile);
    }

   

    IEnumerator DeliverReward(int r)
    { // deliver 
        if (r == 1) // reward left
        {
            arduino.digitalWrite(8, Arduino.HIGH);
            yield return new WaitForSeconds(0.1f);
            arduino.digitalWrite(8, Arduino.LOW);
        }
        else if (r == 2) // reward right
        {
            arduino.digitalWrite(12, Arduino.HIGH);
            yield return new WaitForSeconds(0.1f);
            arduino.digitalWrite(12, Arduino.LOW);
        }
        else { yield return null; };

    }

    IEnumerator FlashCross()
    {
        Debug.Log("flash cross");
        while (true)
        {
            rewardCam.SetActive(true);
            cmd = 4;
            yield return new WaitForSeconds(rDur);
           
            rewardCam.SetActive(false);
            blackCam.SetActive(true);
            float timeout = 5.0f * UnityEngine.Random.value + 10.0f;
            cmd = 3;
            yield return new WaitForSeconds(.5f);
            cmd = 0;
            yield return new WaitForSeconds(timeout);
        }
        
    }

    IEnumerator Punish()
    {   // air puff punishment
        arduino.digitalWrite(11, Arduino.HIGH);
        yield return new WaitForSeconds(0.1f);
        arduino.digitalWrite(11, Arduino.LOW);
    }

}
