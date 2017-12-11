using UnityEngine;
using System;
using System.Collections;
using Uniduino;
using System.IO;
using System.IO.Ports;
using System.Threading;




public class PC_2AFC : MonoBehaviour
{
    public Arduino arduino;

    private GameObject player;
    private GameObject reward;
    private GameObject blackCam;
    private GameObject panoCam;

    private Rigidbody rb;
    private AudioSource sound;
    private AudioSource errSound;

    private SP_2AFC sp;
    private DL_2AFC dl;
    private RR_2AFC rotary;
    private NameCheck nc;
    

    private bool reward_dir;


    private Vector3 initialPosition;
    private Vector3 movement;

    private static bool created = false;
    private int r;
    private int r_last = 0;

    private int RPort = 10;
    private int LPort = 11;
    private int puff = 12;

    public int cmd = 3;
    private bool flashFlag = false;
    private float LastRewardTime;
    private int prevReward = 0;

    public ArrayList LickHistory;

    public void Awake()
    {

        //
        GameObject player = GameObject.Find("Player");
        sp = player.GetComponent<SP_2AFC>();
        rotary = player.GetComponent<RR_2AFC>();
        dl = player.GetComponent<DL_2AFC>();
        
        
        panoCam = GameObject.Find("panoCamera");
        panoCam.transform.eulerAngles = new Vector3(0.0f, -90.0f, 0.0f);
        reward = GameObject.Find("Reward");
        initialPosition = new Vector3(0f, 6f, -50.0f);

        sound = player.GetComponent<AudioSource>();
        errSound = GameObject.Find("basic_maze").GetComponent<AudioSource>();

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
        // get game objects
        arduino = Arduino.global;
        arduino.Setup(ConfigurePins);
        // put the mouse in the dark tunnel

        reward.transform.position = reward.transform.position + new Vector3(0.0f, 0.0f, sp.mrd + UnityEngine.Random.value * sp.ard); ;

    }

    void ConfigurePins()
    {   // lickports
        arduino.pinMode(11, PinMode.OUTPUT); // R - sweetened condensed milk 
        arduino.pinMode(10, PinMode.OUTPUT); // L - water or .5 mM diluted quinine
        arduino.pinMode(12, PinMode.OUTPUT);
        arduino.pinMode(3, PinMode.PWM); // LED

        Debug.Log("Pins configured (player controller)");
    }

    void Update()
    {
        // make sure rotation angle is 0
        transform.eulerAngles = new Vector3(0.0f, 90.0f, 0.0f);

        // end game after appropriate number of trials
        if ((sp.numTraversals >= sp.numTrialsTotal) | (sp.numRewards >= sp.maxRewards & transform.position.z < 0f))
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }

        if (dl.r > 0 & dl.rflag < 1) { StartCoroutine(DeliverReward(dl.r)); dl.rflag = 1; }; // deliver appropriate reward

        // manual rewards and punishments
        if (Input.GetKeyDown(KeyCode.Q) | Input.GetMouseButtonDown(0)) // reward left - sweetened condensed milk
        {
            StartCoroutine(DeliverReward(11));

            if (sp.saveData)
            {
                var sw = new StreamWriter(sp.manRewardFile, true);
                sw.Write(Time.realtimeSinceStartup + "\t" + 1f + "\r\n");
                sw.Close();
            }
        }

        if (Input.GetKeyDown(KeyCode.P) | Input.GetMouseButtonDown(1)) // reward right - quinine
        {

            StartCoroutine(DeliverReward(12));

            if (sp.saveData)
            {
                var sw = new StreamWriter(sp.manRewardFile, true);
                sw.Write(Time.realtimeSinceStartup + "\t" + 2f + "\r\n");
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
            int side = (int)sp.morph + 1;
            var sw = new StreamWriter(sp.rewardFile, true);
            sw.Write(transform.position.z + "\t" + Time.realtimeSinceStartup + "\t" + sp.morph + "\t" + side + "\r\n");
            sw.Close();

            StartCoroutine(RewardSequence(side));
            StartCoroutine(MoveReward());

        }
        else if (other.tag == "Teleport")
        {
            sp.numTraversals += 1;

            sound.Stop();
            transform.position = initialPosition;
            StartCoroutine(InterTrialTimeout());
            reward.transform.position = new Vector3(0.0f, 0.0f, sp.mrd + UnityEngine.Random.value * sp.ard);
            LastRewardTime = Time.realtimeSinceStartup; // to avoid issues with teleports
        }

    }

    IEnumerator InterTrialTimeout()
    {
        rotary.toutBool = 0f;
        if (prevReward == 0) // omission
        {
            yield return new WaitForSeconds(7f);

        }
        else if (prevReward == 1)
        {// correct
            yield return new WaitForSeconds(.5f);
        }
        else if (prevReward == 2)
        {
            yield return new WaitForSeconds(7f);
        }
        rotary.toutBool = 1f;
        prevReward = 0;
    }


    IEnumerator MoveReward()
    {
        reward.transform.position = reward.transform.position + new Vector3(0f, 0f, 1000f);
        yield return null;
    }

    void OnApplicationQuit()
    {
        arduino.analogWrite(3, 0);
        panoCam.SetActive(false);

    }

    IEnumerator RewardSequence(int side)
    {   // water reward
        arduino.analogWrite(3, 20); // turn LED on
        yield return new WaitForSeconds(.5f);
        if (side == 1)
        {
            cmd = 1;
        }
        else if (side == 2)
        {
            cmd = 2;
        }
        //cmd = 7; // dispense on first side that is licked
        yield return new WaitForSeconds(sp.rDur);
        arduino.analogWrite(3, 0); // turn LED off
        yield return new WaitForSeconds(.05f);
        cmd = 3;
        yield return new WaitForSeconds(.5f);
        cmd = 0;

    }



    IEnumerator LightsOn()
    {
        if (sound.isPlaying != true)
        {
            sound.Play();
        }
        // switch to pano cam to active and make sure lights are on
        panoCam.SetActive(true);


        yield return null;
    }


    IEnumerator DeliverReward(int r)
    { // deliver
        if (r == 1) // reward left
        {
            prevReward = 1;
            LickHistory.Add(.33f);
            arduino.digitalWrite(LPort, Arduino.HIGH);
            yield return new WaitForSeconds(0.01f);
            arduino.digitalWrite(LPort, Arduino.LOW);
            sp.numRewards += 1;
            Debug.Log(sp.numRewards);
            prevReward = 1;
        }
        else if (r == 2) // reward right
        {
            prevReward = 1;
            LickHistory.Add(.66f);
            arduino.digitalWrite(RPort, Arduino.HIGH);
            yield return new WaitForSeconds(0.01f);
            arduino.digitalWrite(RPort, Arduino.LOW);
            sp.numRewards += 1;
            Debug.Log(sp.numRewards);
            


        }
        else if (r == 3)
        {
            prevReward = 2;
            LickHistory.Add(.33f);
            errSound.Play();
            arduino.digitalWrite(puff, Arduino.HIGH);
            yield return new WaitForSeconds(0.1f);
            arduino.digitalWrite(puff, Arduino.LOW);
            yield return new WaitForSeconds(0.5f);

            errSound.Stop();
            yield return new WaitForSeconds(0.5f);
            
        }
        else if (r == 4)
        {
            prevReward = 2;
            LickHistory.Add(.66f);
            errSound.Play();
            arduino.digitalWrite(puff, Arduino.HIGH);
            yield return new WaitForSeconds(0.1f);
            arduino.digitalWrite(puff, Arduino.LOW);
            yield return new WaitForSeconds(0.5f);

            errSound.Stop();
            yield return new WaitForSeconds(0.5f);
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
        else
        {
            if (sp.morph == 0)
            {
                LickHistory.Add(.66f);
            }
            else if (sp.morph == 1)
            {
                LickHistory.Add(.33f);
            }

            yield return null;
        };

        if (LickHistory.Count > 20)
        {
            LickHistory.RemoveAt(0);
        }

    }

}

