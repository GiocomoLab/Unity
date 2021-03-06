﻿using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.IO.Ports;
using System.Threading;

public class PC_FlashLED : MonoBehaviour
{

    public float r_timeout = 5.0f; // timeout duration between available rewards
   

    private static int numRewards = 0;
    private int numRewards_manual = 0;
    private int rewardFlag = 0;

    // for saving data
    private SP_FlashLED sp;
    private GameObject player;
    private DL_FlashLED dl;

    public bool pcntrl_pins = false;
    private bool reward_dir;
    private bool flashFlag;

    private static bool created = false;
    private int r;
    private int r_last = 0;



    public int cmd = 0;
    public bool gng = false;
    public bool noClick = false;
    private float lastLick;
    public int LEDOn = 0;
    public int mRewardFlag = 0;
    
    public void Awake()
    {
        // get game objects 
        GameObject player = GameObject.Find("Player");
        sp = player.GetComponent<SP_FlashLED>();
        dl = player.GetComponent<DL_FlashLED>();
    }


    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.F)) & (!flashFlag)) { StartCoroutine(FlashLED()); flashFlag = true; };

        
        if (dl.r > 0) { StartCoroutine(DeliverReward(dl.r)); dl.r = 0; }; // deliver appropriate reward 

        // manual rewards and punishments
        mRewardFlag = 0;
        if (Input.GetKeyDown(KeyCode.Q) | Input.GetMouseButtonDown(0)) // reward left
        {
            mRewardFlag = 0;
            numRewards_manual += 1;
            Debug.Log(numRewards_manual);
            StartCoroutine(DeliverReward(4));
        }

    }

    // save manipulation data to server
    void OnApplicationQuit()
    {
       

    }

    IEnumerator FlashLED()
    {
        while (true)
        {
            LEDOn = 1;
            cmd = 7;
            yield return new WaitForSeconds(0.01f);
            if (noClick)
            {
                cmd = 12;
            }
            else
            {
                cmd = 1;
            }
                
            yield return new WaitForSeconds(sp.rDur);
            cmd = 2;
            LEDOn = 0;
            lastLick = Time.realtimeSinceStartup;
            yield return new WaitForSeconds(.5f);
            cmd = 0;
            sp.numFlashes++;

            float timeout = 5.0f * UnityEngine.Random.value + 5.0f;
            //yield return new WaitForSeconds(timeout);

            if (gng)
            {
                while (true)
                {
                    if (dl.c_1 > 0)
                    {
                        lastLick = Time.realtimeSinceStartup;
                        yield return null;
                    }
                    else
                    {
                        yield return null;
                        if ((Time.realtimeSinceStartup - lastLick)>5.0f)
                        {
                            break;
                        }
                        
                    }
                }
            }
            else
            {
                yield return new WaitForSeconds(timeout);
            }
            

        }
    }

    IEnumerator DeliverReward(int r)
    { // deliver 
        if (r == 1) // reward
        {
            sp.numRewards += 1;
            
        } else if (r==4)
        {
            cmd = 4;
            yield return new WaitForSeconds(.05f);
            cmd = 0;
        }
        else { yield return null; };

    }

}
