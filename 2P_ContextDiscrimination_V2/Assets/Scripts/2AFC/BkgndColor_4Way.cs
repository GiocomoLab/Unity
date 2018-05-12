using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BkgndColor_4Way : MonoBehaviour
{

    private SP_4Way sp;
    private float morph;
    private float cxt;
    private GameObject player;

    private Color bkgnd;
    private Camera[] cameras;
    

    // Use this for initialization
    void Start()
    {
        player = GameObject.Find("Player");
        sp = player.GetComponent<SP_4Way>();



        cameras = GetComponentsInChildren<Camera>();
        foreach (var cam in cameras)
        {

            cam.clearFlags = CameraClearFlags.SolidColor;
            
        }
        StartCoroutine(drawBackgroundColor());

    }

    // Update is called once per frame
    void Update()
    {
        
        foreach (var cam in cameras)
        {
            if (sp.cxt <= .5f)
            {
                cam.backgroundColor = Color.Lerp(Color.black, Color.white, sp.morph);
            } else
            {
                cam.backgroundColor = Color.Lerp(Color.black, Color.white, .5f);
            }

        }
        
    }


    IEnumerator drawBackgroundColor()
    {
        //Camera[] cameras = GetComponentsInChildren<Camera>();
        //float i = Random.Range(0.0f, 1.0f);
        bkgnd.r = morph; bkgnd.b = morph; bkgnd.g = morph; bkgnd.a = 0.0f;

        foreach (var cam in cameras)
        {

            //cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = bkgnd;

        }
        yield return null;
    }
}
