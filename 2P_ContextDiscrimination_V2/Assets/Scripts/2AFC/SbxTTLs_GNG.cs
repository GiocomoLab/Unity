using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class SbxTTLs_GNG : MonoBehaviour {

   
   
    
    private PC_GNG pc;
    private int numTraversals_local = -1;
    //private int numTraversals;
   
    private SP_GNG sp;
    private string timesyncFile;
    private string serverTimesyncFile;
    public bool scanning = false;
    public bool stop = false;

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
        sp = player.GetComponent<SP_GNG>();
        pc = player.GetComponent<PC_GNG>();
        Debug.Log(sp.numTraversals);

        

        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S) & !scanning) {
            StartCoroutine(ScannerStart());   
        };
        
        if ((Input.GetKeyDown(KeyCode.B) & scanning) | (stop & scanning))
        {
            StartCoroutine(ScannerStart());
        }

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
        pc.cmd = 8;
        yield return new WaitForSeconds(.01f);
        pc.cmd = 0;
    }

    IEnumerator ScannerStart()
    {

        //start first trial ttl1
        if (scanning)
        {
            scanning = false;
        }
        else
        {
            scanning = true;
        }

        pc.cmd = 8;
        yield return new WaitForSeconds(.01f);
        pc.cmd = 0;
        yield return new WaitForSeconds(10f);
        pc.cmd = 9;
        yield return new WaitForSeconds(.01f);
        pc.cmd = 0;
        Debug.Log("Press G to Start!");

        
   }

    

}
