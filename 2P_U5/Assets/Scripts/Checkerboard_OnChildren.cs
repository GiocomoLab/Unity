using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkerboard_OnChildren : MonoBehaviour {

    private int dim1 = 5;
    private int dim2 = 5;
    

    private SessionParams_2AFC paramsScript;
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
        paramsScript = player.GetComponent<SessionParams_2AFC>();
        morph = paramsScript.morph;

        // find walls


        StartCoroutine(drawCheckerboard());

    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator drawCheckerboard()
    {

        Texture2D texture = new Texture2D(dim1, dim2);
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (var r in renderers)
        {
            // Do something with the renderer here...
            r.material.mainTexture = texture; // like disable it for example. 
        }


        
        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {


                if (UnityEngine.Random.value<=0.5f) { 
                    intensity = 1;
                          
                } else
                {
                    intensity = 0;
                };
                

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
