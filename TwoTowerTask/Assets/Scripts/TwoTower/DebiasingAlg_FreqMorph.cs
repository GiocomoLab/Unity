using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebiasingAlg_FreqMorph : MonoBehaviour {

    private SP_TwoTower sp;
    private PC_TwoTower pc;
    private GameObject player;
    private int switchFlag = 0;
    private float rand_val;
    private int numTraversalsLocal = -1;
    private int nextMorphTrial=-1;
    private int morphCounter = 0;

    
    public int numTrainingTrials = 100;
    public int numTestTrials = 0;
    public float morphExp = 3f;



    private float[] testTrials;
    private float[] morphVals;
    private float[] Pr_vec;
    private float[] P_corrector_vec;
    private ArrayList trialHistory;

    private void Awake()
    {
        player = GameObject.Find("Player");
        sp = player.GetComponent<SP_TwoTower>();
        pc = player.GetComponent<PC_TwoTower>();

        Pr_vec = new float[]
        {
            .0061f, .0068f,  .0074f, .0081f, .0089f, .0097f, .0105f, .0114f, 0.0124f, .0133f, .0143f,
            .0154f, .0165f, .0176f, .0188f, .020f, .0212f, .0224f, .0237f, .0249f, .0261f, .0274f,
            .0286f, .0298f, .031f, .0321f, .0332f, .0343f, .0353f, .0362f, .0371f, .0379f, .0386f,
            .0392f, .0398f, .0402f, .0406f, .0409f, .0410f, .0411f
        };

        P_corrector_vec = new float[]
        {
            .0217f, .0219f, .0222f, .0224f, .0226f, .0228f, .0230f, .0232f, .0234f, .0236f, .0238f, .0240f,
            .0242f, .0244f, .0246f, .0247f, .0249f, .0251f, .0252f, .0253f, .255f, .0256f, .0257f, .0259f,
            .0260f, .0261f, .0262f, .0263f, .0263f, .0264f, .0265f, .0266f, .0266f, .0267f, .0267f, .0267f,
            .0268f, .0268f, .0268f, .0268f
        };

        trialHistory = new ArrayList();
        for (int i=0; i<40; i++)
        {
            trialHistory.Add(.5f);
        };
        sp.numTrialsTotal =numTrainingTrials + numTestTrials;
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

            sp.morph = NextTrial();
            Debug.Log("next trial");
            trialHistory.Add(sp.morph);
            

            if (trialHistory.Count>40)
            {
                trialHistory.RemoveAt(0);
            }
        }
    }

    float NextTrial()
    {
        float e0 = 0f; float e1 = 0f; int i = 0;

        while (pc.LickHistory.Count > 40)
        {
            pc.LickHistory.RemoveAt(0);
        }

        //int i;
        Debug.Log(i);
        Debug.Log(pc.LickHistory.Count);
        if (pc.LickHistory.Count<40)
        {
            for (int j = 0; j<40-pc.LickHistory.Count; j++)
            {
                e1 = e1 + .5f * Pr_vec[j];
                e0 = e0 + .5f * Pr_vec[j];
                
            }
            i = 40-pc.LickHistory.Count;
        }
        else
        {
            i = 0;
        }

        foreach (float j in pc.LickHistory)
        {
            //Debug.Log(i);
            if (j == -1)
            {
                Debug.Log(i);
                e1 = e1 + Pr_vec[i];
            }
            else if (j == 1)
            {
                Debug.Log(i);
                e0 = e0 + Pr_vec[i];
            }
            
            i++;
         }


        float p1; 
        if (e0+e1 == 0f)
        {
            p1 = .5f;
        } else
        {
            p1 =Mathf.Sqrt(e1) / (Mathf.Sqrt(e1) + Mathf.Sqrt(e0));
        }
        
        if (p1>.85f) { p1 = .85f; } else if (p1<.15f) { p1 = .15f; };
        Debug.Log(p1);
        float sum = 0; int k = 0;
        foreach (float j in trialHistory)
        {
            sum = sum + j*P_corrector_vec[k];
            k++;
        }
        float emp_p1 = sum / (float) trialHistory.Count;

        float thresh;
        if (p1 > emp_p1)
        {
            thresh = .5f * p1;
        }
        else
        {
            thresh = .5f * (1 + p1);
        }


        if (RandomFromDistribution.RandomRangeLinear(0f, 1f,0f) < p1)
        {
            return RandomFromDistribution.RandomRangeLinear(.5f, 1f,0f);
           // return 1f;
        } else
        {
            return RandomFromDistribution.RandomRangeLinear(0f, .5f, 0f);
            //return 0f;
        }

    }

    float[] FisherYates(float[] origArray)
        {
        // then shuffle values (Fisher-Yates shuffle)
        int[] order = new int[origArray.Length];

        for (int i = 0; i < origArray.Length; i++)
        {
            order[i] = i;
        }


        for (int i = order.Length - 1; i >= 0; i--)
        {
            int r = (int)UnityEngine.Mathf.Round(UnityEngine.Random.Range(0, i));
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
