﻿using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.IO.Ports;
using System.Threading;

public class PC_LEDCue_2Port : MonoBehaviour
{

    public float r_timeout = 5.0f; // timeout duration between available rewards
   

    private static int numRewards = 0;
    private int numRewards_manual = 0;
    private int rewardFlag = 0;

    // for saving data
    private SP_LEDCue_2Port sp;
    private GameObject player;
    private DL_LEDCue_2Port dl;

    public bool pcntrl_pins = false;
    private bool reward_dir;
    private bool flashFlag;

    private static bool created = false;
    private int r;
    private int r_last = 0;

    public int cmd = 0;

    public ArrayList LickHistory;

    public void Awake()
    {
        // get game objects 
        GameObject player = GameObject.Find("Player");
        sp = player.GetComponent<SP_LEDCue_2Port>();
        dl = player.GetComponent<DL_LEDCue_2Port>();

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
        // initialize arduino
        cmd = 4;
    }

    void ConfigurePins()
    {   // lickports
        Debug.Log("Pins configured (player controller)");
        pcntrl_pins = true;
    }

    void Update()
    {

        cmd = 14;
        if (dl.r > 0) { StartCoroutine(DeliverReward(dl.r)); dl.r = 0; }; // deliver appropriate reward 


        
        // manual rewards and punishments
        if (Input.GetKeyDown(KeyCode.Q) | Input.GetMouseButtonDown(0)) // reward left
        {
            numRewards_manual += 1;
            Debug.Log(numRewards_manual);
            StartCoroutine(DeliverReward(7));

            
            var sw = new StreamWriter(sp.rewardFile, true);
            sw.Write(Time.realtimeSinceStartup + "\t" + 1f + "\r\n");
            sw.Close();
            
        }

        if (Input.GetKeyDown(KeyCode.P) | Input.GetMouseButtonDown(1)) // reward right
        {
            numRewards_manual += 1;
            Debug.Log(numRewards_manual);
            StartCoroutine(DeliverReward(8));
            
            var sw = new StreamWriter(sp.rewardFile, true);
            sw.Write(Time.realtimeSinceStartup + "\t" + 2f + "\r\n");
            sw.Close();
            
        }

        r_last = dl.r;

    }

    // save manipulation data to server
    void OnApplicationQuit()
    {
        

    }




    IEnumerator DeliverReward(int r)
    { // deliver

        if (r == 1) // reward left
        {

            LickHistory.Add(.25f); // .33f);
            sp.numRewards += 1;
            //Debug.Log(sp.numRewards);
            // prevReward = 1;
        }
        else if (r == 2) // reward right
        {
            //prevReward = 1;
            LickHistory.Add(.75f); // .66f);
            sp.numRewards += 1;
            // Debug.Log(sp.numRewards);



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
        { }


        if (LickHistory.Count > 10)
        {
            LickHistory.RemoveAt(0);
        }

    }

}
