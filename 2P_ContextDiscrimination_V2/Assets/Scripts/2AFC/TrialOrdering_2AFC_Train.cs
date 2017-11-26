using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TrialOrdering_2AFC_Train : MonoBehaviour
{

    private SP_2AFC sp;
    private PC_2AFC pc;
    private SlideTableServoCOM_2AFC servoport;
    private GameObject player;
    private int switchFlag = 0;
    private float rand_val;
    private int numTraversalsLocal = -1;
    private float thresh = 0.5f;

    private void Awake()
    {
        player = GameObject.Find("Player");
        sp = player.GetComponent<SP_2AFC>();
        pc = player.GetComponent<PC_2AFC>();
        servoport = player.GetComponent<SlideTableServoCOM_2AFC>();


    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (player.transform.position.z < 0 & numTraversalsLocal != sp.numTraversals)
        {
            numTraversalsLocal++;
            servoport.servoFlag = true;
            Debug.Log(pc.LickHistory[0]);
            if (pc.LickHistory.Count >= 20)
            {
                float sum = 0;
                foreach (float j in pc.LickHistory)
                {
                    sum += j;
                }
                thresh = sum / (float)pc.LickHistory.Count;

            }
            if (UnityEngine.Random.value <= thresh)
            {
                sp.morph = 0f;
            }
            else
            {
                sp.morph = 1f;
            }
            var sw = new StreamWriter(sp.trialOrderFile, true);
            sw.Write(sp.morph + "\t" + "0" + "\r\n");
            sw.Close();

        }

    }
}
