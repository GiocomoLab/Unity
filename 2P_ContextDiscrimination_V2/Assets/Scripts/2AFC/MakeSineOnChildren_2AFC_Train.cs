using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeSineOnChildren_2AFC_Train : MonoBehaviour
{

    private int dim1 = 3600;
    private int dim2 = 450;
    private float f1 = 3;
    private float f2 = .5f;
    private float theta1 = 0;
    private float theta2 = 60;

    private SP_2AFC sp;
    private float morph;

    private Color color;
    private Renderer eastRenderer;
    private Renderer westRenderer;
    

    private GameObject player;
    private GameObject blackCam;
    private GameObject eWall;
    private GameObject wWall;

    private RR_2AFC rr;
    private TrialOrdering_2AFC_Train trialOrder;

    private int numTraversalsLocal = -1;

    // Use this for initialization
    void Start()
    {
        player = GameObject.Find("Player");
        sp = player.GetComponent<SP_2AFC>();
        //blackCam = GameObject.Find("Black Camera");
        rr = player.GetComponent<RR_2AFC>();
        trialOrder = player.GetComponent<TrialOrdering_2AFC_Train>();

        eWall = GameObject.Find("East Wall");
        eastRenderer = eWall.GetComponent<Renderer>();
        wWall = GameObject.Find("West Wall");
        westRenderer = wWall.GetComponent<Renderer>();

        

        morph = sp.morph;

    }

    // Update is called once per frame
    void Update()
    {
        if (numTraversalsLocal != sp.numTraversals | morph != sp.morph)
        {
            numTraversalsLocal = sp.numTraversals;

            morph = sp.morph;

            rr.speedBool = 0;
            StartCoroutine(drawSineWall());
            
            rr.speedBool = 1;
        }
    }


    IEnumerator drawSineWall()
    {

        Texture2D texture = new Texture2D(dim1, dim2);
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (var r in renderers)
        {
            // Do something with the renderer here...
            r.material.mainTexture = texture; // like disable it for example. 
        }



        float theta = morph * theta1 + (1 - morph) * theta2;
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