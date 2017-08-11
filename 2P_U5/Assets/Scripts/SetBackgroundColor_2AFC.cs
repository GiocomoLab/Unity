using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetBackgroundColor_2AFC : MonoBehaviour {

    private SessionParams_2AFC paramsScript;
    private float morph;
    private GameObject player;

    private Color bkgnd;
    private Camera[] cameras;
    //private Camera cam;

    // Use this for initialization
    void Start()
    {
        player = GameObject.Find("Player");
        paramsScript = player.GetComponent<SessionParams_2AFC>();
        morph = paramsScript.morph;

        

        cameras = GetComponentsInChildren<Camera>();
        foreach (var cam in cameras)
        {

            cam.clearFlags = CameraClearFlags.SolidColor;
            //cam.backgroundColor = bkgnd;
        }
        StartCoroutine(drawBackgroundColor());

    }

    // Update is called once per frame
    void Update()
    {
        //if (cameras[1].backgroundColor.r != morph)
            //(player.transform.position.z <= 0.0f & morph != paramsScript.morph)
        //{
          //  Debug.Log(morph);
            //Debug.Log(paramsScript.morph);
           // morph = paramsScript.morph;
            //bkgnd.r = morph; bkgnd.b = morph; bkgnd.g = morph; bkgnd.a = 0.0f;
            foreach (var cam in cameras)
            {

                //cam.clearFlags = CameraClearFlags.SolidColor;
                cam.backgroundColor = Color.Lerp(Color.black,Color.white,paramsScript.morph);

            }
           // StartCoroutine(drawBackgroundColor());
  //      }
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
