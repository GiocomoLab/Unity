﻿using UnityEngine;
using System;
using System.Collections;
using Uniduino;
using System.IO;
using System.IO.Ports;
using System.Threading;




public class PC_OneSided : MonoBehaviour
{
   

    private GameObject player;
    private GameObject reward;
    private GameObject blackCam;
    private GameObject panoCam;

    private Rigidbody rb;
    private AudioSource sound;
    private AudioSource errSound;

    private SP_OneSided sp;
    private DL_OneSided dl;
    private RR_OneSided rotary;
    private NameCheck nc;

    private bool reward_dir;


    private Vector3 initialPosition;
    private Vector3 movement;

    private static bool created = false;
    private int r;
    private int r_last = 0;

    //private int RPort = 9;
    //private int LPort = 11;
    //private int puff = 12;
    private int prevReward = 0;

    public int cmd = 3;
    private bool flashFlag = false;
    private float LastRewardTime;

    public ArrayList LickHistory;

    public void Awake()
    {
       
        //
        GameObject player = GameObject.Find("Player");
        sp = player.GetComponent<SP_OneSided>();
        rotary = player.GetComponent<RR_OneSided>();
        dl = player.GetComponent<DL_OneSided>();
        blackCam = GameObject.Find("Black Camera");
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
        // put the mouse in the dark tunnel
       
        reward.transform.position = reward.transform.position + new Vector3(0.0f, 0.0f, sp.mrd + UnityEngine.Random.value * sp.ard); ;

    }

   

    void Update()
    {
        // make sure rotation angle is 0
        transform.eulerAngles = new Vector3(0.0f, 90.0f, 0.0f);

        // end game after appropriate number of trials
        if ((sp.numTraversals >= sp.numTrialsTotal) | (sp.numRewards >= sp.maxRewards   & transform.position.z < 0f ))
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }

        if (dl.r > 0 & dl.rflag < 1) { StartCoroutine(DeliverReward(dl.r)); dl.rflag = 1; }; // deliver appropriate reward

        // manual rewards and punishments
        if (Input.GetKeyDown(KeyCode.Q) | Input.GetMouseButtonDown(0)) // reward left - sweetened condensed milk
        {
            StartCoroutine(DeliverReward(7));

           
            var sw = new StreamWriter(sp.manRewardFile, true);
            sw.Write(Time.realtimeSinceStartup + "\t" + 1f + "\r\n");
            sw.Close();
            
    }

        if (Input.GetKeyDown(KeyCode.P) | Input.GetMouseButtonDown(1)) // reward right - quinine
        {

            StartCoroutine(DeliverReward(8));

            
            var sw = new StreamWriter(sp.manRewardFile, true);
            sw.Write(Time.realtimeSinceStartup + "\t" + 2f + "\r\n");
            sw.Close();
            
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
            int side = (int) sp.morph + 1;
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
            yield return new WaitForSeconds(8f);

        }
        else if (prevReward == 1)
        {// correct
            yield return new WaitForSeconds(.5f);
        }
        else if (prevReward == 2) {
            yield return new WaitForSeconds(8f);
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
        
        panoCam.SetActive(false);

    }

    IEnumerator RewardSequence(int side)
    {   // water reward
        cmd = 11;
        yield return new WaitForSeconds(.5f);
        
        if (side ==1)
        {
            if (sp.sceneName == "OneSidedCues_AnyLick")
            {
                cmd = 5;
            } else {
                cmd = 1;
            }
            
        } else if (side == 2)
        {
            if (sp.sceneName == "OneSidedCues_AnyLick")
            {
                cmd = 6;
            }
            else
            {
                cmd = 2;
            }
        }
        
        yield return new WaitForSeconds(sp.rDur);
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

            LickHistory.Add(.25f); // .33f);
            sp.numRewards += 1;
            //Debug.Log(sp.numRewards);
            prevReward = 1;
        }
        else if (r == 2) // reward right
        {
            prevReward = 1;
            LickHistory.Add(.75f); // .66f);
            sp.numRewards += 1;
            // Debug.Log(sp.numRewards);



        }
        else if (r == 3)
        {
            if (sp.morph == 1.0f)
            {
                prevReward = 2;
                LickHistory.Add(.25f); //.33f);
                errSound.Play();
                yield return new WaitForSeconds(0.5f);
                errSound.Stop();
            }
            else
            {
                prevReward = 1;
            }


        }
        else if (r == 4)
        {
            if (sp.morph == 0f)
            {
                prevReward = 2;
                LickHistory.Add(.75f); // 66f);
                errSound.Play();
                yield return new WaitForSeconds(0.5f);
                errSound.Stop();
            }
            else
            {
                prevReward = 1;
            }

        }
        else if (r == 7)
        {
            cmd = 7;
            yield return new WaitForSeconds(0.01f);
            cmd = 0;
        }
        else if (r == 8)
        {
            cmd = 8;
            yield return new WaitForSeconds(0.01f);
            cmd = 0;
        }
        else
        {
            if (sp.morph == 0)
            {
                LickHistory.Add(.75f); //.66f);
            }
            else if (sp.morph == 1)
            {
                LickHistory.Add(.25f); //.33f);
            }

            yield return null;
        };

        if (LickHistory.Count > 20)
        {
            LickHistory.RemoveAt(0);
        }

    }

}
