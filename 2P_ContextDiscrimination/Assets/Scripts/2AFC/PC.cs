using UnityEngine;
using System;
using System.Collections;
using Uniduino;
using System.IO;
using System.IO.Ports;
using System.Threading;




public class PC : MonoBehaviour
{
    public Arduino arduino;

    private GameObject player;
    private GameObject reward;
    private GameObject blackCam;
    private GameObject panoCam;

    private Rigidbody rb;
    private AudioSource sound;

    private SP sp;
    private DetectLicks_2Port dl;
    private ReadRotary rotary;
    private LinAct actuator;
    private NameCheck nc;

    private bool reward_dir;


    private Vector3 initialPosition;
    private Vector3 movement;

    private static bool created = false;
    private int r;
    private int r_last = 0;

    public int cmd = 3;
    private bool flashFlag = false;
    private float LastRewardTime;

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
        

        //
        GameObject player = GameObject.Find("Player");
        sp = player.GetComponent<SP>();
        rotary = player.GetComponent<ReadRotary>();
        dl = player.GetComponent<DetectLicks_2Port>();
        actuator = player.GetComponent<LinAct>();
        blackCam = GameObject.Find("Black Camera");
        panoCam = GameObject.Find("panoCamera");
        panoCam.transform.eulerAngles = new Vector3(0.0f, -90.0f, 0.0f);
        reward = GameObject.Find("Reward");
        initialPosition = new Vector3(0f, 0.5f, -50.0f);
        
        sound = GameObject.Find("basic_maze").GetComponent<AudioSource>();

        LastRewardTime = Time.realtimeSinceStartup;


    }

    void Start()
    {
        // get game objects
        arduino = Arduino.global;
        arduino.Setup(ConfigurePins);
        // put the mouse in the dark tunnel
        StartCoroutine(LightsOff());
        reward.transform.position  = reward.transform.position + new Vector3(0.0f, 0.0f, sp.mrd + UnityEngine.Random.value * sp.ard); ;

    }

    void ConfigurePins()
    {   // lickports
        arduino.pinMode(11, PinMode.OUTPUT); // R
        arduino.pinMode(10, PinMode.OUTPUT); // L
        arduino.pinMode(9, PinMode.OUTPUT); // airpuff
        arduino.pinMode(3, PinMode.PWM); // LED

        Debug.Log("Pins configured (player controller)");
    }

    void Update()
    {
        // make sure rotation angle is 0
        transform.eulerAngles = new Vector3(0.0f, 90.0f, 0.0f);

        // end game after appropriate number of trials
        if (transform.position.z <= 0 & sp.numTraversals == sp.numTrialsTotal)
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }

        if (dl.r > 0 & dl.rflag < 1) { StartCoroutine(DeliverReward(dl.r)); dl.rflag = 1; }; // deliver appropriate reward

        // manual rewards and punishments
        if (Input.GetKeyDown(KeyCode.Q) | Input.GetMouseButtonDown(0)) // reward left
        {
            StartCoroutine(DeliverReward(1));

            if (sp.saveData)
            {
                var sw = new StreamWriter(sp.manRewardFile, true);
                sw.Write(rotary.delta_z + "\t" + Time.realtimeSinceStartup + "\t" + -1.0f + "\t" + 1f + "\r\n");
                sw.Close();
            }
        }

        if (Input.GetKeyDown(KeyCode.P) | Input.GetMouseButtonDown(1)) // reward right
        {
            
            StartCoroutine(DeliverReward(2));

            if (sp.saveData)
            {
                var sw = new StreamWriter(sp.manRewardFile, true);
                sw.Write(rotary.delta_z + "\t" + Time.realtimeSinceStartup + "\t" + -1.0f + "\t" + 2f + "\r\n");
                sw.Close();
            }
        }

        if (Input.GetKeyDown(KeyCode.Backspace)) // punish
        {
            StartCoroutine(Punish());
            if (sp.saveData)
            {
                var sw = new StreamWriter(sp.manRewardFile, true);
                sw.Write(rotary.delta_z + "\t" + Time.realtimeSinceStartup + "\t" + -1.0f + "\t" + 5f + "\r\n");
                sw.Close();
            }
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
            sp.numRewards += 1;

            StartCoroutine(MoveReward());
            //movement = new Vector3(0.0f, 0.0f, sp.mrd + UnityEngine.Random.value * sp.ard);
            //other.gameObject.transform.position = other.gameObject.transform.position + movement;


        }
        else if (other.tag == "Teleport")
        {
            sp.numTraversals += 1;
            StartCoroutine(LightsOff());
            sound.Stop();
            transform.position = initialPosition;
            reward.transform.position = new Vector3(0.0f, 0.0f, sp.mrd + UnityEngine.Random.value * sp.ard);
            LastRewardTime = Time.realtimeSinceStartup; // to avoid issues with teleports
        }

    }

    IEnumerator MoveReward()
    {
        float CurrRewardTime = Time.realtimeSinceStartup;

        if (!sp.fixedRewardSchedule)
        {

            if (CurrRewardTime - LastRewardTime > 20.0f)
            {
                sp.mrd = Mathf.Max(sp.MinTrainingDist, sp.mrd - 10f);

            }
            else
            {
                sp.mrd = Mathf.Min(sp.MaxTrainingDist, sp.mrd + 10f);
            }
        }
        reward.transform.position = reward.transform.position + new Vector3(0f, 0f, sp.mrd + UnityEngine.Random.value * sp.ard);
        LastRewardTime = CurrRewardTime;
        yield return null;
    }

    void OnApplicationQuit()
    {
        arduino.analogWrite(3, 0);
        actuator.actuatorPort.Write("2");
        blackCam.SetActive(true);
        panoCam.SetActive(false);

    }

    IEnumerator RewardSequence(int side)
    {   // water reward
        arduino.analogWrite(3,20); // turn LED on
        yield return new WaitForSeconds(.5f);
        actuator.actuatorPort.Write("1"); // move forward
        if (side == 1)
        { cmd = 1; }
        else if (side == 2)
        { cmd = 2; };
        yield return new WaitForSeconds(sp.rDur);
        arduino.analogWrite(3,0); // turn LED off
        actuator.actuatorPort.Write("2"); // move port back
        yield return new WaitForSeconds(.05f);
        cmd = 3;
        yield return new WaitForSeconds(.5f);
        cmd = 0;

    }

    IEnumerator LightsOff()
    {
        // switch to black out cam
        blackCam.SetActive(true);
        panoCam.SetActive(false);
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
        
        yield return null;
    }


    IEnumerator DeliverReward(int r)
    { // deliver
        if (r == 1) // reward left
        {
            arduino.digitalWrite(10, Arduino.HIGH);
            yield return new WaitForSeconds(0.05f);
            arduino.digitalWrite(10, Arduino.LOW);
            sp.numRewards += 1;
            Debug.Log(sp.numRewards);
        }
        else if (r == 2) // reward right
        {
            arduino.digitalWrite(11, Arduino.HIGH);
            yield return new WaitForSeconds(0.05f);
            arduino.digitalWrite(11, Arduino.LOW);
            sp.numRewards += 1;
            Debug.Log(sp.numRewards);
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
