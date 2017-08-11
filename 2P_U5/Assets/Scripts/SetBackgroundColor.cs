using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetBackgroundColor : MonoBehaviour {

    private SessionParams_2AFC paramsScript;
    private float morph;

    private Color bkgnd;
    //private Camera cam;

    // Use this for initialization
    void Start()
    {
        GameObject player = GameObject.Find("Player");
        paramsScript = player.GetComponent<SessionParams_2AFC>();
        morph = paramsScript.morph;


        Camera[] cameras = GetComponentsInChildren<Camera>();
        //float i = Random.Range(0.0f, 1.0f);
        bkgnd.r = morph; bkgnd.b = morph; bkgnd.g = morph; bkgnd.a = 0.0f;

        foreach (var cam in cameras)
        {
           
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = bkgnd;
        }
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
