using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickControl_TwoTower : MonoBehaviour {

    
    private SP_TwoTower sp;
    private PC_TwoTower pc;
    private TrialOrdering_TwoTower to;
    private GameObject player;


    private int numTraversalsLocal = -1;
    private float morph = -1;
    private int baseline_half;
    private int baseline_quart;


    void Awake()
    {
        player = GameObject.Find("Player");
        sp = player.GetComponent<SP_TwoTower>();
        pc = player.GetComponent<PC_TwoTower>();
        to = player.GetComponent<TrialOrdering_TwoTower>();

        baseline_half = (int) to.numBaselineTrials/2 ;
        baseline_quart = (int)Mathf.Min(to.numBaselineTrials / 4, 10f);
    }

    // Use this for initialization
    void Start () {
        

    }
	
	// Update is called once per frame
	void Update () {

        if (numTraversalsLocal!= sp.numTraversals)
        {
            numTraversalsLocal = sp.numTraversals;
            if (numTraversalsLocal<to.numBaselineTrials)
            {
                
                if ((numTraversalsLocal<baseline_quart) | ((numTraversalsLocal>=baseline_half) & (numTraversalsLocal<(baseline_half+baseline_quart))))
                {
                    sp.ClickOn = true;
                }
                else if (((numTraversalsLocal>=baseline_quart) & (numTraversalsLocal<baseline_half)) |  (numTraversalsLocal>= (baseline_half+baseline_quart)))
                {
                    if (numTraversalsLocal % 2 == 0)
                    {
                        sp.ClickOn = true;
                    } else
                    {
                        sp.ClickOn = false;
                    }
                    
                }
            }
            else if ((numTraversalsLocal < to.numBaselineTrials+10) & (to.numBaselineTrials>0))
            {
                if (numTraversalsLocal % 2 == 0)
                {
                    sp.ClickOn = true;
                }
                else
                {
                    sp.ClickOn = false;
                }
            }
            else
            {

                if (numTraversalsLocal % 5 == 0)
                {
                    sp.ClickOn = true;
                }
                else
                {
                    sp.ClickOn = false;
                }
               // sp.ClickOn = false;
            }

        }
		
	}
}
