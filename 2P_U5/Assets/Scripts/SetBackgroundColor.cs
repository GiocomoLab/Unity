using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetBackgroundColor : MonoBehaviour {

    private Color purple;
    //private Camera cam;

    // Use this for initialization
    void Start()
    {
        Texture2D texture = new Texture2D(1, 1);
        Camera[] cameras = GetComponentsInChildren<Camera>();
        float i = Random.Range(0.0f, 1.0f);
        purple.r = 1.0f; purple.b = 1.0f; purple.g = 0.0f; purple.a = 0.0f;

        foreach (var cam in cameras)
        {
           
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = Color.Lerp(purple, Color.black,i);
        }
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
