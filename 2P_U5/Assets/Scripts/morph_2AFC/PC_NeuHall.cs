using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using Uniduino;
using System.IO;
using System.IO.Ports;
using System.Threading;


public class PC_NeuHall : MonoBehaviour
{

    public Arduino arduino;
    private Vector3 initialPosition;
    private static int numRewards = 0;
    private int numRewards_manual = 0;
    private int rewardFlag = 0;

    public int numTraversals = 0;
    public int numTrialsTotal;
    public float mrd = 50.0f; // minimum reward distance
    public float ard = 150.0f; // additional reward distance
    public bool punish = false;

    // for saving data
    private string localDirectory;
    private string serverDirectory;
    private SessionParams_2AFC paramsScript;
    private string mouse;
    private string session;
    private string rewardFile;
    private string serverRewardFile;
    private string mRewardFile;
    private string serverMRewardFile;
    //private string lickFile;
    //private string serverLickFile;
    private bool saveData;

    // lights, for visual landmarks
    public Light light1;
    public Light light2;
    private Color initialBackgroundColor;
    private Color initialAmbientLight;

    private Vector3 movement;

    private GameObject player;
    private GameObject reward;
    private GameObject panoCam;
    private GameObject blackCam;
    private GameObject rewardCam;
    private GameObject[] rewards;
    private DetectLicks_2Port_NH dl;

    private Rigidbody rb;

    public bool pcntrl_pins = false;
   
   
    private AudioSource sound;

    private bool reward_dir;

    private static bool created = false;

    public bool pause_player = false;

   
    private int r;

    public int cmd = 0;

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
        initialPosition = new Vector3(0f, 0.5f, -200.0f);

        // initialize arduino
        arduino = Arduino.global;
        arduino.Setup(ConfigurePins);

        // turn off lights
        panoCam = GameObject.Find("panoCamera");
        blackCam = GameObject.Find("Black Camera");
        rewardCam = GameObject.Find("Reward Camera");

        // turn reward camera to proper position
        rewardCam.transform.Rotate(new Vector3(0f, 180f, 0f));

        // put the mouse in the dark tunnel
        StartCoroutine(LightsOff());

        // get session parameters from SessionParams script
        GameObject player = GameObject.Find("Player");
        paramsScript = player.GetComponent<SessionParams_2AFC>();
        numTrialsTotal = paramsScript.numTrialsTotal;

        dl = player.GetComponent<DetectLicks_2Port_NH>();

        // ensure physics is treating the kinematics correctly
        rb = player.GetComponent<Rigidbody>();
        rb.isKinematic = true;

        // setup file names
        localDirectory = paramsScript.localDirectory;
        serverDirectory = paramsScript.serverDirectory;
        rewardFile = localDirectory + "/" + paramsScript.mouse + "/" + paramsScript.session + "_" + paramsScript.sessionType + "_rewards.txt";
        serverRewardFile = serverDirectory + "/" + paramsScript.mouse + "/" + paramsScript.session + "_" + paramsScript.sessionType + "_rewards.txt";
        
        mRewardFile = localDirectory + "/" + paramsScript.mouse + "/" + paramsScript.session + "_" + paramsScript.sessionType + "_MRewards.txt";
        serverMRewardFile = serverDirectory + "/" + paramsScript.mouse + "/" + paramsScript.session + "_" + paramsScript.sessionType + "_MRewards.txt";
        var sw = new StreamWriter(mRewardFile, true);
        sw.Write("");
        sw.Close();

        // find reward and teleport objects
        rewards = GameObject.FindGameObjectsWithTag("Reward");
        sound = GameObject.Find("basic_maze").GetComponent<AudioSource>();

        // place first reward
        movement = new Vector3(0.0f, 0.0f, mrd + UnityEngine.Random.value * ard);
        rewards[0].transform.position = rewards[0].gameObject.transform.position + movement;
        
        
    }

    void ConfigurePins()
    {   // lickports
        arduino.pinMode(11, PinMode.OUTPUT); // R
        arduino.pinMode(8, PinMode.OUTPUT); // L        

        // airpuff solenoid
        arduino.pinMode(12, PinMode.OUTPUT);


        Debug.Log("Pins configured (player controller)");
        pcntrl_pins = true;
    }

    void Update()
    {

        // make sure rotation angle is 0
        transform.eulerAngles = new Vector3(0.0f, 90.0f, 0.0f);

        if (dl.r > 0 & dl.rflag < 1) { StartCoroutine(DeliverReward(dl.r)); dl.rflag = 1; };

        // manual rewards and punishments
        if (Input.GetKeyDown(KeyCode.Q) | Input.GetMouseButtonDown(0)) // reward left
        {
            numRewards_manual += 1;
            Debug.Log(numRewards_manual);
            StartCoroutine(DeliverReward(1));

            var sw = new StreamWriter(mRewardFile, true);
            sw.Write(transform.position.z + "\t" + Time.realtimeSinceStartup + "\t" + -1.0f + "\t" + 1f + "\r\n");
            sw.Close();
        }

        if (Input.GetKeyDown(KeyCode.P) | Input.GetMouseButtonDown(1)) // reward right
        {
            numRewards_manual += 1;
            Debug.Log(numRewards_manual);
            StartCoroutine(DeliverReward(2));

            var sw = new StreamWriter(mRewardFile, true);
            sw.Write(transform.position.z + "\t" + Time.realtimeSinceStartup + "\t" + -1.0f + "\t" + 2f + "\r\n");
            sw.Close();
        }


        if (Input.GetKeyDown(KeyCode.Backspace)) // punish
        {
            StartCoroutine(Punish());
        }
        //

        // end game after appropriate number of trials
        if (transform.position.z <= 0 & numTraversals == numTrialsTotal)
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }

    }

    // save manipulation data to server
    void OnApplicationQuit()
    {
        
        File.Copy(rewardFile, serverRewardFile);
        File.Copy(mRewardFile, serverMRewardFile);
        blackCam.SetActive(true);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.tag);
        if (other.tag == "Start")
        {
            StartCoroutine(LightsOn());
        }
        else if (other.tag == "Reward")
        {
            
            var sw = new StreamWriter(rewardFile, true);
            sw.Write(transform.position.z + "\t" + Time.realtimeSinceStartup + "\t" + paramsScript.morph + "\t" + -1f + "\r\n");
            sw.Close();

            StartCoroutine(RewardScreen());
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

    IEnumerator RewardScreen()
    {   // water reward
        
        pause_player = true;
        panoCam.SetActive(false);
        rewardCam.SetActive(true);

        cmd = 7;
        yield return new WaitForSeconds(1.5f);
        panoCam.SetActive(true);
        rewardCam.SetActive(false);
        yield return new WaitForSeconds(.05f);
        cmd = 3;
        pause_player = false;
        yield return new WaitForSeconds(.5f);
        cmd = 0;

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
        else if (r == 5) // no reward
        {
            if (punish) { StartCoroutine(Punish()); };
            yield return null;
        } else { yield return null; };

    }

    IEnumerator Punish()
    {   // air puff punishment
        arduino.digitalWrite(11, Arduino.HIGH);
        yield return new WaitForSeconds(0.1f);
        arduino.digitalWrite(11, Arduino.LOW);
    }


    IEnumerator LightsOff()
    {
        // switch to black out cam
        blackCam.SetActive(true);
        panoCam.SetActive(false);
        rewardCam.SetActive(false);
        yield return null;
    }

    IEnumerator LightsOn()
    {
        
        // switch to pano cam to active and make sure lights are on
        panoCam.SetActive(true);
        blackCam.SetActive(false);
        rewardCam.SetActive(false);
        //light1.enabled = true;
        //light2.enabled = true;
        RenderSettings.ambientLight = initialAmbientLight;
        yield return null;
    }

   
    IEnumerator EnableAllRewards()
    {
        foreach (GameObject item in GameObject.FindGameObjectsWithTag("Reward"))
        {
            Collider collider = item.GetComponent<Collider>();
            collider.enabled = true;
        }
        yield return null;
    }



   
    


}
