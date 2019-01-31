using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ForagingControl_TwoTower : MonoBehaviour {

    private GameObject reward;
    private GameObject player;
    private SP_TwoTower sp;
    private PC_TwoTower pc;
    private Vector3 initialPosition;



    private int numTraversalsLocal = -1;
    

    // Use this for initialization
    void Start () {
        player = GameObject.Find("Player");
        sp = player.GetComponent<SP_TwoTower>();
        pc = player.GetComponent<PC_TwoTower>();
        

        reward = GameObject.Find("Reward");
    }
	
	// Update is called once per frame
	void Update () {
        if (numTraversalsLocal != sp.numTraversals)
        {
            numTraversalsLocal = sp.numTraversals;
            if (UnityEngine.Random.value>sp.ForagingSkipTrialPcnt)
            {
                reward.SetActive(true);
                reward.transform.position = new Vector3(0f,0f,250.0f + 150.0f * UnityEngine.Random.value); ;
            } else
            {
                reward.SetActive(false);
            }
            

        }
    }
}
