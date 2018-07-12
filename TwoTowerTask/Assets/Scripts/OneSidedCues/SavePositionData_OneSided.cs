using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class SavePositionData_OneSided : MonoBehaviour
{

    private SP_OneSided sp;
    private RR_OneSided rr;

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
        sp = player.GetComponent<SP_OneSided>();
        rr = player.GetComponent<RR_OneSided>();

    }

    void Update()
    {
        // write position data to file every frame
       
            var sw = new StreamWriter(sp.posFile, true);
            sw.Write(transform.position.z + "\t" + sp.morph + "\t" + Time.realtimeSinceStartup + "\t" + sp.morph + "\t" + rr.true_delta_z + "\r\n");
            sw.Close();
        
    }


}
