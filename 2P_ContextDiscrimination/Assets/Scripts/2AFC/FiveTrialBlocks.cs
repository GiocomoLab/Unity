using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiveTrialBlocks : MonoBehaviour {

    private SP sp;
    private PC pc;
    private GameObject player;
    private int switchFlag = 0;
    private float rand_val;
    public int BlockSize = 5;
    private int numTraversalsLocal = 0;

    private void Awake()
    {
        player = GameObject.Find("Player");
        sp = player.GetComponent<SP>();
        pc = player.GetComponent<PC>();
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (player.transform.position.z < 0 & numTraversalsLocal != sp.numTraversals )
        {
            numTraversalsLocal++;
            Debug.Log(numTraversalsLocal % BlockSize);
            if (numTraversalsLocal % 5 == 0) {
                sp.morph = Mathf.Abs(sp.morph - 1f);
                

            }
        }
		
	}
}
