using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TrialOrdering_TwoTower : MonoBehaviour {


    private SP_TwoTower sp;
    private PC_TwoTower pc;
    private GameObject player;
    private int switchFlag = 0;
    private float rand_val;
    private int numTraversalsLocal = -1;

    public int numBaselineTrials = 20;
    public int numTrainingTrials = 100;
    public int numTestTrials = 0;
    public float morphPcnt = .5f;



    public float[] trialOrder;
    private float[] baselineTrials;
    private float[] trainingTrials;
    private float[] testTrials;
    private float[] binaryTrials;
    private float[] morphTrials;
    private float[] morphVals;

    private void Awake()
    {
        player = GameObject.Find("Player");
        sp = player.GetComponent<SP_TwoTower>();
        pc = player.GetComponent<PC_TwoTower>();
        


    }
    // Use this for initialization
    void Start()
    {
        trialOrder = new float[numBaselineTrials + numTrainingTrials + numTestTrials];

        baselineTrials = new float[numBaselineTrials];
        for (int i = 0; i < numBaselineTrials / 2; i++)
        {
            baselineTrials[ i] = 1.0f;
            baselineTrials[ i + numBaselineTrials/2] = 0f;
        }
        //baselineTrials = FisherYates(baselineTrials);

        trainingTrials = new float[numTrainingTrials];
        for (int i = 0; i < numTrainingTrials / 2; i++)
        {
            trainingTrials[2 * i] = 0f;
            trainingTrials[2 * i + 1] = 1f;
        }
        trainingTrials = FisherYates(trainingTrials);

        int numBinaryTrials = (int)(numTestTrials * morphPcnt);
        binaryTrials = new float[numBinaryTrials];
        for (int i = 0; i < numBinaryTrials / 2; i++)
        {
            binaryTrials[2 * i] = 0f;
            binaryTrials[2 * i + 1] = 1.0f;
        }


        //  make and array of morph values
        morphVals = new float[3];
        morphVals[0] = .25f;
        morphVals[1] = .5f;
        morphVals[2] = .75f;
        //morphVals[3] = .67f;
        //morphVals[4] = .84f;

        // permute 5 times and concatenate
        int numMorphTrials = numTestTrials - numBinaryTrials;
        int numRepeats = numMorphTrials / 3;
        morphTrials = new float[numMorphTrials];
        Debug.Log(numMorphTrials);
        for (int i = 0; i < numRepeats; i++) // num repeats
        {
        
            for (int j = 0; j <3; j++) // num morphs
            {
                //Debug.Log(morphVals_perm[j]);
                //Debug.Log(5 * i + j);
                morphTrials[3 * i + j] = morphVals[j];

            }
        }
        Debug.Log(numTestTrials);
        testTrials = new float[numTestTrials];
        for (int i = 0; i < numTestTrials; i++)
        {
            if (i < numBinaryTrials) {
                testTrials[i] = binaryTrials[i];
            }
            else
            {
                testTrials[i] = morphTrials[i - numBinaryTrials];
            }
        }

        testTrials = FisherYates(testTrials);

        // for every 8 trialsS
        sp.numTrialsTotal = numBaselineTrials + numTrainingTrials + numTestTrials;
        trialOrder = new float[sp.numTrialsTotal + 1];
        for (int i = 0; i < numBaselineTrials; i++)
        {
            trialOrder[i] = baselineTrials[i];
        }

        for (int i = 0; i < numTrainingTrials; i++)
        {
            trialOrder[numBaselineTrials + i] = trainingTrials[i];
        }

        for (int i = 0; i<numTestTrials; i++)
        {
            trialOrder[numBaselineTrials + numTrainingTrials + i] = testTrials[i];
        }
    
            
        
        
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
