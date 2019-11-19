using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RewardControl_TwoTower_Timeout : MonoBehaviour {

    private GameObject reward0;
    private GameObject reward1;
    private GameObject timeout0;
    private GameObject timeout1;
    private GameObject player;
    private SP_TwoTower sp;
    private RR_TwoTower rr;
    private PC_TwoTower pc;
    private DebiasingAlg_TwoTower to;

    

    private int numTraversalsLocal = -1;
    private float morph= -1;

    // Use this for initialization
    void Start () {
        player = GameObject.Find("Player");
        sp = player.GetComponent<SP_TwoTower>();
        //blackCam = GameObject.Find("Black Camera");
        rr = player.GetComponent<RR_TwoTower>();
       // to = player.GetComponent<DebiasingAlg_TwoTower>();
        pc = player.GetComponent<PC_TwoTower>();

        reward0 = GameObject.Find("Reward0");
        reward1 = GameObject.Find("Reward1");
        timeout0 = GameObject.Find("Timeout0");
        timeout1 = GameObject.Find("Timeout1");
    }
	
	// Update is called once per frame
	void Update () {
        if (numTraversalsLocal != sp.numTraversals | morph != sp.morph)
        {
            numTraversalsLocal = sp.numTraversals;

            morph = sp.morph;

            

            if (morph + pc.wallJitter + pc.bckgndJitter < .5)
            {
                reward0.SetActive(true);
                reward1.SetActive(false);

                //if (numTraversalsLocal >= to.numBaselineTrials)
                //{
                timeout0.SetActive(true);
                //}
                timeout1.SetActive(false);
            }
            else if (morph + pc.wallJitter + pc.bckgndJitter >= .5)
            {
                reward0.SetActive(false);
                reward1.SetActive(true);

                timeout0.SetActive(false);
                //if (numTraversalsLocal >= to.numBaselineTrials)
                //{
                timeout1.SetActive(true);
                //}
                    
            }
            else
            {
                if (pc.wallJitter + pc.bckgndJitter < 0)
                //if (UnityEngine.Random.value <=.5)
                {
                    reward0.SetActive(true);
                    reward1.SetActive(false);

                    timeout0.SetActive(true);
                    timeout1.SetActive(false);
                }
                else
                {
                    reward0.SetActive(false);
                    reward1.SetActive(true);

                    timeout0.SetActive(false);
                    timeout1.SetActive(true);
                }
            }
            

        }
    }
}
