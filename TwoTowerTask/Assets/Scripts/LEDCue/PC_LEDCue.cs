using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.IO.Ports;
using System.Threading;

public class PC_LEDCue : MonoBehaviour {

    public float rDur = 2.0f; // timeout duration between available rewards

    private static int numRewards = 0;
    private int numRewards_manual = 0;
    private int rewardFlag = 0;

    // for saving data
    private SP_LEDCue sp;
    private GameObject player;
    private DetectLicks_1Port_LEDCue dl;
 
    public bool pcntrl_pins = false;
    private bool reward_dir;

    private static bool created = false;
    private int r;
    private int r_last = 0;

    public int cmd = 0;

    
    public void Awake()
    {
        // get game objects 
        GameObject player = GameObject.Find("Player");
        sp = player.GetComponent<SP_LEDCue>();
        dl = player.GetComponent<DetectLicks_1Port_LEDCue>();
    }

    private void Start()
    {
        cmd = 10;
    }
    void Update()
    {

        //cmd = 10;
        if (dl.r > 0) { StartCoroutine(DeliverReward(dl.r)); dl.r = 0; }; // deliver appropriate reward 

        // manual rewards and punishments
        if (Input.GetKeyDown(KeyCode.Q) | Input.GetMouseButtonDown(0)) // reward left
        {
            numRewards_manual += 1;
            Debug.Log(numRewards_manual);
            StartCoroutine(DeliverReward(4));

            var sw = new StreamWriter(sp.manRewardFile, true);
            sw.Write(Time.realtimeSinceStartup + "\r\n");
            sw.Close();
            
        }

    }

    // save manipulation data to server
    void OnApplicationQuit()
    {
       
    }


    IEnumerator DeliverReward(int r)
    { // deliver 
        if (r == 4) // reward
        {
            cmd = 4;
            yield return new WaitForSeconds(.05f);
            cmd = 10;
        }
        else { yield return null; };

    }

}
