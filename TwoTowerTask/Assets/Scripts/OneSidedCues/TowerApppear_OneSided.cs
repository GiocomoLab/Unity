using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerApppear_OneSided : MonoBehaviour {
    private SP_OneSided sp;
    private GameObject player;
    private GameObject[] ETowers;
    private GameObject[] WTowers;
    private int NumTraversalsLocal = -1;
    private bool towersOn;
    private float morph;

    private void Awake()
    {
        // find player script
        GameObject player = GameObject.Find("Player");
        sp = player.GetComponent<SP_OneSided>();

        // find east and west towers separately
        ETowers = GameObject.FindGameObjectsWithTag("East Tower");
        WTowers = GameObject.FindGameObjectsWithTag("West Tower");
    }
    // Use this for initialization
    void Start () {
        
        StartCoroutine(SwitchOff()); // make the appropriate towers inactive
        morph = sp.morph;
    }

    // Update is called once per frame
    void Update() {
        // if its a new traversal
        if (NumTraversalsLocal!=sp.numTraversals | morph != sp.morph)
        {
            morph = sp.morph;
            NumTraversalsLocal = sp.numTraversals;
            
                
            StartCoroutine(SwitchOff()); // make the appropriate towers inactive
            

        }
    }

    IEnumerator SwitchOff()
    {
        Debug.Log("Switch off flag");
        if (sp.morph == 0.0f)
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
        else if (sp.morph == 1.0f)
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

}
