using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class SavePositionData_4Way : MonoBehaviour
{

    private SP_4Way sp;
    private RR_4Way rr;
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
        sp = player.GetComponent<SP_4Way>();
        rr = player.GetComponent<RR_4Way>();

    }

    void Update()
    {
        // write position data to file every frame
       
            var sw = new StreamWriter(sp.posFile, true);
            sw.Write(transform.position.z + "\t" + Time.realtimeSinceStartup + "\t" + sp.morph + "\t" + rr.true_delta_z + "\r\n");
            sw.Close();
       
    }


}
