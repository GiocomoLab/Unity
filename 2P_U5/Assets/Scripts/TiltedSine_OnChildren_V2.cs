using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TiltedSine_OnChildren_V2 : MonoBehaviour {

    private int dim1 = 600;
    private int dim2 = 450;
    private float f1 = 1;
    private float f2 = 3;
    private float theta1 = 60;
    private float theta2 = 0;
    
    private Color color;
    private SessionParams_2AFC paramsScript;
    private float morph;

    private Renderer eastRenderer;
    private Renderer westRenderer;

    private GameObject player;

    // Use this for initialization
    void Start () {
        // find walls
        player = GameObject.Find("Player");
        paramsScript = player.GetComponent<SessionParams_2AFC>();
        morph = paramsScript.morph; 


       
        StartCoroutine(drawSineWall());

    }
	
	// Update is called once per frame
	void Update () {
        if (player.transform.position.z<=0.0f & morph!=paramsScript.morph)
        {
            Debug.Log(morph);
            Debug.Log(paramsScript.morph);
            morph = paramsScript.morph;
            StartCoroutine(drawSineWall());
        }
    }

    IEnumerator drawSineWall()
    {

        Texture2D texture = new Texture2D(dim1, dim2);
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (var r in renderers)
        {
            // Do something with the renderer here...
            r.material.mainTexture = null;
            r.material.mainTexture = texture; // like disable it for example. 
        }


        //float phase = 2 * Mathf.PI * Random.Range(0.0f, 1.0f);
        float theta = morph * theta1 + (1-morph) * theta2;
        float f = morph * f1 + (1 - morph) * f2; 
        float thetar = theta * Mathf.PI / 180.0f;
        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                float xs = (float)x / (float)dim2;
                float ys = (float)y / (float)dim1;
                float intensity = Mathf.Cos(2.0f * Mathf.PI * f * (xs * (Mathf.Cos(thetar + Mathf.PI / 4.0f) + Mathf.Sin(thetar + Mathf.PI / 4.0f)) + ys * (Mathf.Cos(thetar + Mathf.PI / 4.0f) - Mathf.Sin(thetar + Mathf.PI / 4.0f))));


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
