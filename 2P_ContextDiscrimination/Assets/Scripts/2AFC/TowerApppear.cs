using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerApppear : MonoBehaviour {
    private SP sp;
    private GameObject player;
    private GameObject[] ETowers;
    private GameObject[] WTowers;
    private int NumTraversalsLocal = -1;
    private bool towersOn;

    private void Awake()
    {
        // find player script
        GameObject player = GameObject.Find("Player");
        sp = player.GetComponent<SP>();

        // find east and west towers separately
        ETowers = GameObject.FindGameObjectsWithTag("East Tower");
        WTowers = GameObject.FindGameObjectsWithTag("West Tower");
    }
    // Use this for initialization
    void Start () {
        // draw a random number 
        float r = UnityEngine.Random.value;
        if (UnityEngine.Random.value>.5f)
        {
            StartCoroutine(SwitchOff()); // make the appropriate towers inactive
            towersOn = false;
        } else
        {
            StartCoroutine(SwitchOn());
            towersOn = true;
        }

        
    }

    // Update is called once per frame
    void Update() {
        // if its a new traversal
        if (NumTraversalsLocal!=sp.numTraversals)
        {
            
            NumTraversalsLocal++;
            // switch each run
            if (towersOn == true)
            {
                
                StartCoroutine(SwitchOff()); // make the appropriate towers inactive
                towersOn = false;
            }
            else
            {
                StartCoroutine(SwitchOn());
                towersOn = true;
            }


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
        } else
        {
            foreach (GameObject wt in WTowers)
            {
                wt.SetActive(true);
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
