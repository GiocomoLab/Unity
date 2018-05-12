using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TrialOrdering_Test : MonoBehaviour {


    private SP_2AFC sp;
    private PC_2AFC pc;
    private SlideTableCOM_2AFC servoport;
    private GameObject player;
    private int switchFlag = 0;
    private float rand_val;
    private int numTraversalsLocal = -1;
    private float thresh = 0.5f;

    public float[] trialOrder;
    private float[] binaryTrials;
    private float[] morphTrials;
    private float[] morphVals;

    private void Awake()
    {
        player = GameObject.Find("Player");
        sp = player.GetComponent<SP_2AFC>();
        pc = player.GetComponent<PC_2AFC>();
        


    }
    // Use this for initialization
    void Start()
    {
        

        int numBinaryTrials = 210;
        // make an array of 216 binary trials
        binaryTrials = new float[numBinaryTrials];
        // 113 of each 1's and 0's
        for (int i = 0; i < numBinaryTrials/2; i++)
        {
            binaryTrials[2 * i] = 0f;
            binaryTrials[2 * i + 1] = 1.0f;
        }

        //randomly permute
        binaryTrials = FisherYates(binaryTrials);

        //  make and array of morph values
        morphVals = new float[7];
        morphVals[0] = .2f;
        morphVals[1] = .3f;
        morphVals[2] = .4f;
        morphVals[3] = .5f;
        morphVals[4] = .6f;
        morphVals[5] = .7f;
        morphVals[6] = .8f;

        // permute 5 times and concatenate
        morphTrials = new float[35];
        for (int i = 0; i < 5; i++) // num repeats
        {
            float[] morphVals_perm = FisherYates(morphVals);
            for (int j = 0; j<7; j++) // num morphs
            {
                //Debug.Log(morphVals_perm[j]);
                Debug.Log(7 * i + j);
                morphTrials[7 * i + j] = morphVals_perm[j];
                
            }
        }


        // for every 8 trialsS
        sp.numTrialsTotal = 245;
        trialOrder = new float[sp.numTrialsTotal+1];
        int morphCounter = 0;
        int binaryCounter = 0;
        for (int i = 0; i<35; i++)
        {
            // randomly choose which one will be the morph
            int morph_i = (int) UnityEngine.Random.Range(0f, 7f);
            for (int j=0; j<7; j++)
            {
                if (morph_i == j)
                {
                    trialOrder[7 * i + j] = morphTrials[morphCounter];
                    morphCounter++;
                } else
                {
                    trialOrder[7 * i + j] = binaryTrials[binaryCounter];
                    binaryCounter++;
                }
                // Debug.Log(trialOrder[10 * i + j]);
            } 
        }
        //trialOrder[215] = binaryTrials[binaryCounter];
    }

    // Update is called once per frame
    void Update()
    {

        if (player.transform.position.z < 0 & numTraversalsLocal != sp.numTraversals)
        {
            numTraversalsLocal++;
            //servoport.servoFlag = true;
            //Debug.Log(pc.LickHistory[0]);
            sp.morph = trialOrder[numTraversalsLocal];
            Debug.Log(sp.morph);
            
            var sw = new StreamWriter(sp.trialOrderFile, true);
            sw.Write(sp.morph + "\t" + "0" + "\r\n");
            sw.Close();

        }

    }

    float[] FisherYates(float[] origArray)
    {
        // then shuffle values (Fisher-Yates shuffle)
        int[] order = new int[origArray.Length];
        
        for (int i = 0; i< origArray.Length; i++)
        {
            order[i] = i;
        }


        for (int i = order.Length - 1; i >= 0; i--)
        {
            int r = (int) UnityEngine.Mathf.Round(UnityEngine.Random.Range(0, i));
            int tmp = order[i];
            order[i] = order[r];
            order[r] = tmp;
        }

        float[] permArray = new float[origArray.Length];
        for (int i = 0; i < origArray.Length; i++)
        {
            permArray[i] = origArray[order[i]];
        }

        return permArray;
    }
}
