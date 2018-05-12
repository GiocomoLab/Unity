﻿using UnityEngine;
using System.Collections;
using Uniduino;
using System;
using System.IO;

public class SbxTTLs_2AFC : MonoBehaviour {

   
    private int ttl0 = 5;
    private int ttl1 = 4;
    
    private PC_2AFC pc;
    private int numTraversals_local = -1;
    //private int numTraversals;
   
    private SP_2AFC sp;
    private string timesyncFile;
    private string serverTimesyncFile;
    public bool scanning = false;

    private static bool created = false;

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

        // for saving data
        GameObject player = GameObject.Find("Player");
        sp = player.GetComponent<SP_2AFC>();
        pc = player.GetComponent<PC_2AFC>();
        Debug.Log(sp.numTraversals);

        

        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S) & !scanning) {
            StartCoroutine(ScannerStart());   
        };
        
        

        if (Input.GetKeyDown(KeyCode.T) & !scanning)
        {
            StartCoroutine(ScannerToggle());

        };
        
    }

    void OnApplicationQuit()
    {
        
    }

    IEnumerator ScannerToggle()
    {
        pc.cmd = 12;
        yield return new WaitForSeconds(.01f);
        pc.cmd = 0;
    }

    IEnumerator ScannerStart()
    {

        //start first trial ttl1
        scanning = true;
        pc.cmd = 12;
        yield return new WaitForSeconds(.01f);
        pc.cmd = 0;
        yield return new WaitForSeconds(10f);
        pc.cmd = 13;
        yield return new WaitForSeconds(.01f);
        pc.cmd = 0;
        Debug.Log("Press G to Start!");

        
   }

}
