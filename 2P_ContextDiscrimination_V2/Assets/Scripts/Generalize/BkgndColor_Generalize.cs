using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BkgndColor_Generalize : MonoBehaviour
{

    private SP_2AFC sp;
    private float morph;
    private GameObject player;

    private Color bkgnd;
    private Color f1;
    private Color f2; 
    private Camera[] cameras;
    

    // Use this for initialization
    void Start()
    {
        player = GameObject.Find("Player");
        sp = player.GetComponent<SP_2AFC>();

        f2.r = .8f; f2.g = .8f; f2.b = .8f;
        f1.r = .2f; f1.g = .2f; f1.b = .2f;

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
            cam.backgroundColor = Color.Lerp(f1, f2, sp.morph);

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
