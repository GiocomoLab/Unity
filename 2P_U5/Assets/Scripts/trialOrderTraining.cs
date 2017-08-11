using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trialOrderTraining : MonoBehaviour {

    private SessionParams_2AFC paramsScript;
    private PC_2AFC playerController;
    private GameObject player;
    private int switchFlag = 0;

    // Use this for initialization
    void Start () {
        player = GameObject.Find("Player");
        paramsScript = player.GetComponent<SessionParams_2AFC>();
        playerController = player.GetComponent<PC_2AFC>();
    }
	
	// Update is called once per frame
	void Update () {

        if (player.transform.position.z<0f & playerController.numTraversals==Mathf.Ceil(paramsScript.numTrialsTotal/2) & switchFlag == 0)
            {
            switchFlag = 1;
            paramsScript.morph = Mathf.Abs(paramsScript.morph - 1f);
            }
		
	}
}
