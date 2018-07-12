using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class SavePositionData_TwoTower : MonoBehaviour
{

    private SP_TwoTower sp;
    private RR_TwoTower rr;
    private static bool created = false;

    public void Awake()
    {
        if (!created)
        {
            // this is the first instance - make it persist
            DontDestroyOnLoad(this);
            created = true;
        }
        else
        {
            // this must be a duplicate from a scene reload - DESTROY!
            Destroy(this);
        }
    }

    void Start()
    {
        GameObject player = GameObject.Find("Player");
        sp = player.GetComponent<SP_TwoTower>();
        rr = player.GetComponent<RR_TwoTower>();
    }

    void Update()
    {
        // write position data to file every frame
       
        
    }


}
