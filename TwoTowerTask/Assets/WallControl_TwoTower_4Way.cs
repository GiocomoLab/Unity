using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallControl_TwoTower_4Way : MonoBehaviour {
    private GameObject SineWalls;
    private GameObject BlockWalls;
    private GameObject player;
    private SP_TwoTower sp;
    private RR_TwoTower rr;
    private DebiasingAlg_TwoTower to;



    private int numTraversalsLocal = -1;
    private float morph = -1;
    private bool bw = false;

    // Use this for initialization
    void Start()
    {
        player = GameObject.Find("Player");
        sp = player.GetComponent<SP_TwoTower>();
        rr = player.GetComponent<RR_TwoTower>();
        to = player.GetComponent<DebiasingAlg_TwoTower>();

        SineWalls = GameObject.Find("SineWalls");
        BlockWalls = GameObject.Find("BlockWalls");
        
    }

    // Update is called once per frame
    void Update()
    {
        if (numTraversalsLocal != sp.numTraversals | morph != sp.morph | bw != sp.BlockWalls)
        {
            numTraversalsLocal = sp.numTraversals;

            morph = sp.morph;

            bw = sp.BlockWalls;


            if (bw)
            {
                SineWalls.SetActive(false);
                BlockWalls.SetActive(true);

            }
            else
            {
                SineWalls.SetActive(true);
                BlockWalls.SetActive(false);
            }

            


        }
    }
}
