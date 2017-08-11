using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XOrPeriodizeTexture_OnChildren : MonoBehaviour {

    private int dim1 = 1200;
    private int dim2 = 450;
    private int xf1 = 2;
    private int xf2 = 5;
    private int yf1 = 1;
    private int yf2 = 3;

    private SessionParams paramsScript;
    private float morph;
    
    
    private Color color;
    //private Component playerScript;
    private Renderer eastRenderer;
    private Renderer westRenderer;
    private float intensity;

    // Use this for initialization
    void Start()
    {
        GameObject player = GameObject.Find("Player");
        paramsScript = player.GetComponent<SessionParams>();
        morph = paramsScript.morph;

        // find walls


        StartCoroutine(drawXOr());

    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator drawXOr()
    {

        Texture2D texture = new Texture2D(dim1, dim2);
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (var r in renderers)
        {
            // Do something with the renderer here...
            r.material.mainTexture = texture; // like disable it for example. 
        }


        int xf = (int)(morph * (float)xf1 + (1 - morph) * (float)xf2);
        int yf = (int)(morph * (float)yf1 + (1 - morph) * (float)yf2);

        int xperiod = dim1 / xf ;
        int yperiod = dim2 / yf ;
        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {


                int xr = x%xperiod ;
                int yr = y%yperiod ;
                

                if (yr<yperiod/2)
                {
                    if (xr < xperiod /2)
                    {
                        intensity = 1;
                        //print(xr);
                        
                    } else
                    {
                        intensity = 0;
                    }
                } else
                {
                    if (xr < (xperiod /2))
                    {
                        intensity = 0;
                    } else
                    {
                        intensity = 1;
                    }
                }
                

                color.r = intensity;
                color.g = intensity;
                color.b = intensity;


                color.a = 0.0f; // set alpha to 0 (transparency... reduces glare)
                texture.SetPixel(x, y, color);
            }
        }
        texture.filterMode = FilterMode.Point;
        texture.Apply();

        yield return null;
    }
}
