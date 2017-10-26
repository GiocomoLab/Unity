using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiasCorrectionOrdering : MonoBehaviour {

    private SP sp;
    private PC pc;
    private GameObject player;
    private int switchFlag = 0;
    private float rand_val;
    private int numTraversalsLocal = -1;
    private float thresh = 0.5f;

    private void Awake()
    {
        player = GameObject.Find("Player");
        sp = player.GetComponent<SP>();
        pc = player.GetComponent<PC>();


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
            Debug.Log(pc.LickHistory[0]);
            if (pc.LickHistory.Count >= 20)
            {
                float sum = 0;
                foreach (float j in pc.LickHistory)
                {
                    sum += j;
                }
                thresh = sum / (float)pc.LickHistory.Count;

            }
            if (UnityEngine.Random.value <= thresh)
            {
                sp.morph = 0f;
            } else
            {
                sp.morph = 1f;
            }
           
        }

    }
}
