using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class SavePositionData : MonoBehaviour
{

    private SP sp;
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
        sp = player.GetComponent<SP>();
        
    }

    void Update()
    {
        // write position data to file every frame
        if (sp.saveData)
        {
            var sw = new StreamWriter(sp.posFile, true);
            sw.Write(transform.position.z + "\t" + Time.realtimeSinceStartup + "\r\n");
            sw.Close();
        }
    }

   
}
