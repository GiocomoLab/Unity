using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerAppear_HalfOneSided : MonoBehaviour
{
    private SP_OneSided sp;
    private GameObject player;
    private GameObject[] ETowers;
    private GameObject[] WTowers;
    private int NumTraversalsLocal = -1;
    private bool towersOn;
    private TrialOrdering_HalfOneSided trialOrder;
    private float morph;

    private void Awake()
    {
        // find player script
        GameObject player = GameObject.Find("Player");
        sp = player.GetComponent<SP_OneSided>();
        trialOrder = player.GetComponent<TrialOrdering_HalfOneSided>();

        // find east and west towers separately
        ETowers = GameObject.FindGameObjectsWithTag("East Tower");
        WTowers = GameObject.FindGameObjectsWithTag("West Tower");
    }
    // Use this for initialization
    void Start()
    {
        morph = sp.morph;
        //StartCoroutine(SwitchOff()); // make the appropriate towers inactive

    }

    // Update is called once per frame
    void Update()
    {
        // if its a new traversal
        if (NumTraversalsLocal != sp.numTraversals | morph != sp.morph)
        {

            NumTraversalsLocal = sp.numTraversals;
            morph = sp.morph;
           
            if (trialOrder.oddTwoSided )
            {
                if ((NumTraversalsLocal + 1) % 2 == 1)
                {
                    StartCoroutine(SwitchOn());
                } else
                {
                    StartCoroutine(SwitchOff()); // make the appropriate towers inactive
                }

            } else
            {
                if ((NumTraversalsLocal + 1) % 2 == 1)
                {
                    StartCoroutine(SwitchOff());
                }
                else
                {
                    StartCoroutine(SwitchOn()); // make the appropriate towers inactive
                }
            }


            


        }
    }

    IEnumerator SwitchOff()
    {
        Debug.Log("Switch off flag");
        if (morph == 0.0f)
        {

            foreach (GameObject wt in WTowers)
            {

                wt.SetActive(true);
            }
            foreach (GameObject et in ETowers)
            {
                et.SetActive(false);
            }
        }
        else if (morph == 1.0f)
        {
            foreach (GameObject wt in WTowers)
            {
                wt.SetActive(false);
            }
            foreach (GameObject et in ETowers)
            {

                et.SetActive(true);
            }
        }

        yield return null;
    }
    IEnumerator SwitchOn()
    {
        foreach (GameObject wt in WTowers)
        {
            wt.SetActive(true);
        }
        foreach (GameObject et in ETowers)
        {
            et.SetActive(true);
        }
        yield return null;
    }
}

