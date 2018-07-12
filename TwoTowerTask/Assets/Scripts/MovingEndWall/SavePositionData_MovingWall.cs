using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class SavePositionData_MovingWall : MonoBehaviour
{

    private SP_MovingWall sp;
    private RR_MovingWall rr;
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
        sp = player.GetComponent<SP_MovingWall>();
        rr = player.GetComponent<RR_MovingWall>();
        
    }

    void Update()
    {
        // write position data to file every frame
        
        var sw = new StreamWriter(sp.posFile, true);
        sw.Write(transform.position.z + "\t" + Time.realtimeSinceStartup + "\t" + rr.true_delta_z + "\r\n");
        sw.Close();
        
    }

   
}
