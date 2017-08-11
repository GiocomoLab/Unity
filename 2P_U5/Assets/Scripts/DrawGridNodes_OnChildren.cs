using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawGridNodes_OnChildren : MonoBehaviour {

    private int dim1 = 1200;
    private int dim2 = 600;
    private float f2 = 1;
    private float f1 = 5;
    private SessionParams_2AFC paramsScript;
    private float morph;

    private Color color;
    //private Component playerScript;
    private Renderer eastRenderer;
    private Renderer westRenderer;

    private GameObject player;

    // Use this for initialization
    void Start()
    {
        player = GameObject.Find("Player");
        paramsScript = player.GetComponent<SessionParams_2AFC>();
        morph = paramsScript.morph;

        // find walls
        //Renderer[] renderers = GetComponentsInChildren<Renderer>();

        StartCoroutine(drawGridNodes());

    }

    // Update is called once per frame
    void Update()
    {
        if (player.transform.position.z <= 0.0f & morph != paramsScript.morph)
        {
            
            morph = paramsScript.morph;
            StartCoroutine(drawGridNodes());
        }
    }

    IEnumerator drawGridNodes()
    {

        Texture2D texture = new Texture2D(dim1, dim2);
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (var r in renderers)
        {
            // Do something with the renderer here...
            r.material.mainTexture = texture; // like disable it for example. 
        }



        float f = morph * f1 + (1 - morph) * f2;
        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                float xs = (float)x / (float)dim2;
                float ys = (float)y / (float)dim1;

                float sin1 = Mathf.Cos(2.0f * Mathf.PI * f * (xs * (Mathf.Cos(Mathf.PI / 4.0f) + Mathf.Sin(Mathf.PI / 4.0f)) + ys * (Mathf.Cos(Mathf.PI / 4.0f) - Mathf.Sin(Mathf.PI / 4.0f))));
                float sin2 = Mathf.Cos(2.0f * Mathf.PI * f * (xs * (Mathf.Cos(2.0f * Mathf.PI / 3.0f + Mathf.PI / 4.0f) + Mathf.Sin(2.0f * Mathf.PI / 3.0f + Mathf.PI / 4.0f)) + ys * (Mathf.Cos(2.0f * Mathf.PI / 3.0f + Mathf.PI / 4.0f) - Mathf.Sin(2.0f*Mathf.PI/3.0f + Mathf.PI / 4.0f))));
                float sin3 = Mathf.Cos(2.0f * Mathf.PI * f * (xs * (Mathf.Cos(4.0f * Mathf.PI / 3.0f + Mathf.PI / 4.0f) + Mathf.Sin(4.0f * Mathf.PI / 3.0f + Mathf.PI / 4.0f)) + ys * (Mathf.Cos(4.0f * Mathf.PI / 3.0f + Mathf.PI / 4.0f) - Mathf.Sin(4.0f * Mathf.PI / 3.0f + Mathf.PI / 4.0f))));

                float intensity = sin1 + sin2 + sin3;


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
