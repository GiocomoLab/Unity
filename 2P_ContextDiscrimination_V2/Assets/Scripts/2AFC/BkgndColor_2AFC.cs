using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BkgndColor_2AFC : MonoBehaviour
{

    private SP_2AFC sp;
    private float morph;
    private GameObject player;

    private Color bkgnd;
    private Camera[] cameras;
    

    // Use this for initialization
    void Start()
    {
        player = GameObject.Find("Player");
        sp = player.GetComponent<SP_2AFC>();



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
            cam.backgroundColor = Color.Lerp(Color.black, Color.white, sp.morph);

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
