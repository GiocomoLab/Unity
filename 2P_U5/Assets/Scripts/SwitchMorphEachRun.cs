using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchMorphEachRun: MonoBehaviour
{

    private SessionParams_2AFC paramsScript;
    private PC_2AFC playerController;
    private GameObject player;
    private int switchFlag = 0;
    private float rand_val;

    // Use this for initialization
    void Start()
    {
        player = GameObject.Find("Player");
        paramsScript = player.GetComponent<SessionParams_2AFC>();
        playerController = player.GetComponent<PC_2AFC>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (player.transform.position.z < 0f & switchFlag == 0 )
        {
            rand_val = UnityEngine.Random.value;
            
            Debug.Log(rand_val);
            switchFlag = 1;
            if (rand_val <= .5f)
            {
                paramsScript.morph = Mathf.Abs(paramsScript.morph - 1f);
            }

            
        }
        if (player.transform.position.z> 0f && switchFlag ==1)
        {
            switchFlag = 0;

        }
    }
}

