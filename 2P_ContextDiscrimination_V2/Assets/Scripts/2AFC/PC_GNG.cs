using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.IO.Ports;
using System.Threading;




public class PC_GNG : MonoBehaviour
{
    

    private GameObject player;
    private GameObject reward;
    private GameObject blackCam;
    private GameObject panoCam;

    private Rigidbody rb;
    //private AudioSource sound;
    //private AudioSource errSound;

    private SP_GNG sp;
    private DL_GNG dl;
    private RR_GNG rotary;
    private NameCheck nc;
    private TrialOrdering_GNG to;


    private bool reward_dir;


    private Vector3 initialPosition;
    private Vector3 movement;

    private static bool created = false;
    private int r;
    private int r_last = 0;

    public int cmd = 2;
    private bool flashFlag = false;
    private float LastRewardTime;
    private int prevReward = 0;
    private float prevMorph = .5f;

    public ArrayList LickHistory;

    public void Awake()
    {

        //
        //Application.targetFrameRate = 100;
        
        GameObject player = GameObject.Find("Player");
        sp = player.GetComponent<SP_GNG>();
        rotary = player.GetComponent<RR_GNG>();
        dl = player.GetComponent<DL_GNG>();
        to = player.GetComponent<TrialOrdering_GNG>();



        panoCam = GameObject.Find("panoCamera");
        panoCam.transform.eulerAngles = new Vector3(0.0f, -90.0f, 0.0f);
        reward = GameObject.Find("Reward");
        initialPosition = new Vector3(0f, 6f, -50.0f);

       // sound = player.GetComponent<AudioSource>();
       // errSound = GameObject.Find("basic_maze").GetComponent<AudioSource>();

        LickHistory = new ArrayList();
        int j = 0;
        while (j <  20)
        {
            LickHistory.Add(0.5f);
            j++;
        }
        //Debug.Log(LickHistory[0]);

    }

    void Start()
    {

        // put the mouse in the dark tunnel
        cmd = 0;
        reward.transform.position = reward.transform.position + new Vector3(0.0f, 0.0f, sp.mrd + UnityEngine.Random.value * sp.ard); ;

    }

    void Update()
    {
        // make sure rotation angle is 0
        transform.eulerAngles = new Vector3(0.0f, 90.0f, 0.0f);

        // end game after appropriate number of trials
        if ((sp.numTraversals >= sp.numTrialsTotal) | (sp.numRewards >= sp.maxRewards & transform.position.z < 0f))
        {
            UnityEditor.EditorApplication.isPlaying = false;
            //Application.Quit();
        }

        if (dl.r > 0 & dl.rflag < 1) { StartCoroutine(DeliverReward(dl.r)); dl.rflag = 1; }; // deliver appropriate reward

        // manual rewards and punishments
        if (Input.GetKeyDown(KeyCode.Q) | Input.GetMouseButtonDown(0)) // reward left - sweetened condensed milk
        {
            StartCoroutine(DeliverReward(7));
            //cmd = 7;
            
            var sw = new StreamWriter(sp.manRewardFile, true);
            sw.Write(Time.realtimeSinceStartup + "\t" + 1f + "\r\n");
            sw.Close();
            
        }

        



    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.tag);
        if (other.tag == "Start")
        {
            //sound.Play();
            StartCoroutine(LightsOn());
        }

        else if (other.tag == "Reward")
        {
            int side;
            if (sp.numTraversals < to.numBaselineTrials)
            {
                side = 1;
            }
            else
            {
                if (sp.morph < 0.5)
                {
                    side = 1;
                }
                else if (sp.morph > .5)
                {
                    side = 2;
                }
                else
                {
                    if (UnityEngine.Random.value < .5)
                    {
                        side = 1;
                    }
                    else
                    {
                        side = 2;
                    }


                }
            }
            
            var sw = new StreamWriter(sp.rewardFile, true);
            sw.Write(transform.position.z + "\t" + Time.realtimeSinceStartup + "\t" + sp.morph + "\t" + side + "\r\n");
            sw.Close();

            StartCoroutine(RewardSequence(side));
            StartCoroutine(MoveReward());

        }
        else if (other.tag == "Teleport")
        {
            sp.numTraversals += 1;

            //sound.Stop();
            transform.position = initialPosition;
            StartCoroutine(InterTrialTimeout());
            reward.transform.position = new Vector3(0.0f, 0.0f, sp.mrd + UnityEngine.Random.value * sp.ard);
            LastRewardTime = Time.realtimeSinceStartup; // to avoid issues with teleports
        }

    }

    IEnumerator InterTrialTimeout()
    {
        rotary.toutBool = 0f;
        cmd = 12;
        yield return new WaitForSeconds(.5f);
        cmd = 0;
        if ((sp.numTraversals == to.numBaselineTrials) | (sp.numTraversals == to.numBaselineTrials + to.numTrainingTrials))
        {
            yield return new WaitForSeconds(120f); // 7f);
        }
        else {
            if (prevReward == 0) // omission
            {
                if (prevMorph < .5)
                {
                    yield return new WaitForSeconds(10f); // 7f);
                }

            }
            else if (prevReward == 1)
            {// correct
                yield return new WaitForSeconds(1f); //.5f);
            }
            else if (prevReward == 2) // incorrect
            {
                yield return new WaitForSeconds(10f); //7f);
            }
            else
            {
                yield return new WaitForSeconds(1f);
            }
        }

        rotary.toutBool = 1f;
        prevReward = 0;
        prevMorph = sp.morph;
    }


    IEnumerator MoveReward()
    {
        reward.transform.position = reward.transform.position + new Vector3(0f, 0f, 1000f);
        yield return null;
    }

    void OnApplicationQuit()
    {
        
        panoCam.SetActive(false);

    }

    IEnumerator RewardSequence(int side)
    {   // water reward
        //arduino.analogWrite(3, 20); // turn LED on
        cmd = 7;
        yield return new WaitForSeconds(.5f);
        if (side == 1)
        {
            cmd = 1;
        }
        else if (side == 2)
        {
            cmd = 11;
        } else
        {
            cmd = 0;
        }
        //cmd = 7; // dispense on first side that is licked
        yield return new WaitForSeconds(sp.rDur);
        cmd = 2;
        yield return new WaitForSeconds(.5f);
        cmd = 0;

    }



    IEnumerator LightsOn()
    {
      
        panoCam.SetActive(true);


        yield return null;
    }


    IEnumerator DeliverReward(int r)
    { // deliver
        
        if (r == 1) // reward left
        {
            sp.numRewards += 1;
            prevReward = 1;
        }
        else if (r == 3) //air puff
        {
            prevReward = 2;
            sp.numRewards += 1;
        } else if (r ==7)
        {
            cmd = 4;
            yield return new WaitForSeconds(0.01f);
            cmd = 0;
        }
       

    }

}

