using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TrialOrdering_4Way : MonoBehaviour {


    private SP_4Way sp;
    private PC_4Way pc;
    private SlideTableCOM_4Way servoport;
    private GameObject player;
    private int switchFlag = 0;
    private float rand_val;
    private int numTraversalsLocal = -1;
    private float thresh = 0.5f;

   

    private void Awake()
    {
        player = GameObject.Find("Player");
        sp = player.GetComponent<SP_4Way>();
        pc = player.GetComponent<PC_4Way>();
        


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
            
            if (pc.LickHistory.Count >= 20)
            {
                float sum = 0;
                foreach (float j in pc.LickHistory)
                {
                    sum += j;
                }
                thresh = sum / (float)pc.LickHistory.Count;

            }
            if (UnityEngine.Random.value <= .5f)
            {
                sp.cxt = 0f;
            } else
            {
                sp.cxt = 1f;
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
            sw.Write(sp.morph + "\t" + sp.cxt + "\r\n");
            sw.Close();

        }


    }

}
