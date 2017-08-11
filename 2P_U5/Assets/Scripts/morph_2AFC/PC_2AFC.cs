using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using Uniduino;
using System.IO;


public class PC_2AFC : MonoBehaviour
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

    private Vector3 movement;

    private GameObject player;
    private GameObject reward;
    private GameObject panoCam;
    private GameObject blackCam;
    private GameObject rewardCam;
    //private GameObject reward;
    private GameObject[] rewards;

    private Rigidbody rb;

    public bool pcntrl_pins = false;
    private bool move_rewards;
    private Scene curr_scene;
    private AudioSource sound;

    private bool reward_dir;

    private static bool created = false;

    public bool pause_player = false;

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
        //arduino.Connect();
        arduino.Setup(ConfigurePins);


        // turn off lights
        panoCam = GameObject.Find("panoCamera");
        blackCam = GameObject.Find("Black Camera");
        rewardCam = GameObject.Find("Reward Camera");


        rewardCam.transform.Rotate(new Vector3(0f, 180f, 0f));
        StartCoroutine(LightsOff());

        // get session parameters from SessionParams script
        GameObject player = GameObject.Find("Player");
        paramsScript = player.GetComponent<SessionParams_2AFC>();
        numTrialsTotal = paramsScript.numTrialsTotal;

        rb = player.GetComponent<Rigidbody>();
        rb.isKinematic = true;

        localDirectory = paramsScript.localDirectory;
        serverDirectory = paramsScript.serverDirectory;
        rewardFile = localDirectory + "/" + paramsScript.mouse + "/" + paramsScript.session + "_rewards.txt";
        serverRewardFile = serverDirectory + "/" + paramsScript.mouse + "/" + paramsScript.session + "_rewards.txt";

        // find reward and teleport objects
        rewards = GameObject.FindGameObjectsWithTag("Reward");
        sound = GameObject.Find("basic_maze").GetComponent<AudioSource>();


        curr_scene = SceneManager.GetActiveScene();
        Debug.Log(curr_scene.name);
        move_rewards = true;


        if (move_rewards)
        {
            movement = new Vector3(0.0f, 0.0f, 50.0f + UnityEngine.Random.value * 150.0f);
            rewards[0].transform.position = rewards[0].gameObject.transform.position + movement;
            //teleport.transform.position = teleport.transform.position + movement;
        }
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

        if (Input.GetKeyDown(KeyCode.Q)) // reward left
        {
            numRewards_manual += 1;
            Debug.Log(numRewards_manual);
            StartCoroutine(Reward(true));
        }

        if (Input.GetKeyDown(KeyCode.P)) // reward right
        {
            numRewards_manual += 1;
            Debug.Log(numRewards_manual);
            StartCoroutine(Reward(false));
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            StartCoroutine(Punish());
        }
        
        // end game after appropriate number of trials
        if (transform.position.z <= 0 & numTraversals == numTrialsTotal)
        {
            //StartCoroutine (sendEmail ()); // send email notification that session is done
            UnityEditor.EditorApplication.isPlaying = false;
        }

    }

    void OnTriggerEnter(Collider other)
    {
        //sound = GameObject.Find("basic_maze").GetComponent<AudioSource>();
        Debug.Log(other.tag);
        if (other.tag == "Start")
        {
            sound.Play();   

            StartCoroutine(LightsOn());

        }
        else if (other.tag == "Reward")
        {
            //pause_player = true;
            reward_dir = UnityEngine.Random.value<=paramsScript.morph;

            var sw = new StreamWriter(rewardFile, true);
            sw.Write(transform.position.z + "\t" + Time.realtimeSinceStartup + "\t" + paramsScript.morph + "\t" + reward_dir + "\r\n");
            sw.Close();

            StartCoroutine(Reward(reward_dir));

            

            numRewards += 1;
            
            if (move_rewards)
            {
                movement = new Vector3(0.0f, 0.0f, mrd + UnityEngine.Random.value*ard);
                other.gameObject.transform.position = other.gameObject.transform.position + movement ;
                //teleport.transform.position = teleport.transform.position + movement;
            }
            else
            {
                other.enabled = false;
            }

            //Debug.Log ("Rewards: " + numRewards);

        }
        else if (other.tag == "Teleport")
        {
            numTraversals += 1;
            StartCoroutine(LightsOff());
            sound.Stop();
            transform.position = initialPosition;

            if (move_rewards)
            {
                // get componenet reward, then move

                rewards[0].transform.position = new Vector3(0.0f, 0.0f, 50.0f);
                //teleport.transform.position = rewards[0].transform.position + new Vector3(0.0f, 0.0f, 10.0f);
            }
            else
            {
                // enable all rewards
                StartCoroutine(EnableAllRewards());
                //SceneManager.LoadScene("CueRich2"); // for debugging, 

            }
            //other.enabled = true;

        }

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
        if (sound.isPlaying != true)
        {
            sound.Play();
        }
        // switch to pano cam to active and make sure lights are on
        panoCam.SetActive(true);
        blackCam.SetActive(false);
        rewardCam.SetActive(false);
        //light1.enabled = true;
        //light2.enabled = true;
        RenderSettings.ambientLight = initialAmbientLight;
        yield return null;
    }

    IEnumerator Reward(bool side)
    {   // water reward
        // true = left, false = right
        pause_player = true;
        panoCam.SetActive(false);
        rewardCam.SetActive(true);

        

        yield return new WaitForSeconds(0.5f);
        if (side)
        {
            arduino.digitalWrite(8, Arduino.HIGH);
            yield return new WaitForSeconds(0.1f);
            arduino.digitalWrite(8, Arduino.LOW);
        }  else 
        {
            arduino.digitalWrite(12, Arduino.HIGH);
            yield return new WaitForSeconds(0.1f);
            arduino.digitalWrite(12, Arduino.LOW);
        };
        yield return new WaitForSeconds(0.5f);
        panoCam.SetActive(true);
        rewardCam.SetActive(false);
        yield return new WaitForSeconds(.05f);
        pause_player = false;


    }

    IEnumerator Punish()
    {   // air puff punishment
        arduino.digitalWrite(11, Arduino.HIGH);
        yield return new WaitForSeconds(0.1f);
        arduino.digitalWrite(11, Arduino.LOW);
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

    

    // save manipulation data to server
    void OnApplicationQuit()
    {
        blackCam.SetActive(true);
        if (saveData)
        {
            File.Copy(rewardFile, serverRewardFile);
        }
    }

}
