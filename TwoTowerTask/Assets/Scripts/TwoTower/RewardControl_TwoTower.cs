using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RewardControl_TwoTower : MonoBehaviour {

    private GameObject reward0;
    private GameObject reward1;
    private GameObject player;
    private SP_TwoTower sp;
    private PC_TwoTower pc;
    

    

    private int numTraversalsLocal = -1;
    private float morph= -1;

    // Use this for initialization
    void Start () {
        player = GameObject.Find("Player");
        sp = player.GetComponent<SP_TwoTower>();
        pc = player.GetComponent<PC_TwoTower>();
        

        reward0 = GameObject.Find("Reward0");
        reward1 = GameObject.Find("Reward1");
    }
	
	// Update is called once per frame
	void Update () {
        if (numTraversalsLocal != sp.numTraversals | morph != sp.morph)
        {
            numTraversalsLocal = sp.numTraversals;

            morph = sp.morph;

            

            if (morph<.5)
            {
                reward0.SetActive(true);
                reward1.SetActive(false);
            }
            else if (morph > .5)
            {
                reward0.SetActive(false);
                reward1.SetActive(true);
            }
            else
            {
                if (pc.wallJitter + pc.bckgndJitter < 0)
                //if (UnityEngine.Random.value <=.5)
                {
                    reward0.SetActive(true);
                    reward1.SetActive(false);
                }
                else
                {
                    reward0.SetActive(false);
                    reward1.SetActive(true);
                }
            }
            

        }
    }
}
