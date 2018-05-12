using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallControl_4Way : MonoBehaviour
{

    private int sin_dim1 = 3600;
    private int sin_dim2 = 450;
    private float sin_f1 = 2; //3;
    private float sin_f2 = 1; //.5f;
    private float sin_theta1 = 10;
    private float sin_theta2 = 60;



    private int block_dim1 = 50;
    private int block_dim2 = 5;


    private float block_th1 = .1f;
    private float block_th2 = .9f;
    private float block_thresh;


    private SP_4Way sp;
    private float morph = -1f;
    private float cxt = -1f;

    private Color color;
    private Renderer eastRenderer;
    private Renderer westRenderer;
    

    private GameObject player;
    private GameObject blackCam;
    private GameObject eWall;
    private GameObject wWall;

    private RR_4Way rr;
    private float intensity;
    //  private TrialOrdering_Test trialOrder;

    private int numTraversalsLocal = -1;

    // Use this for initialization
    void Start()
    {
        player = GameObject.Find("Player");
        sp = player.GetComponent<SP_4Way>();
        //blackCam = GameObject.Find("Black Camera");
        rr = player.GetComponent<RR_4Way>();
//       trialOrder = player.GetComponent<TrialOrdering_Test>();

        eWall = GameObject.Find("East Wall");
        eastRenderer = eWall.GetComponent<Renderer>();
        wWall = GameObject.Find("West Wall");
        westRenderer = wWall.GetComponent<Renderer>();

        

        morph = sp.morph;
        cxt = sp.cxt;

    }

    // Update is called once per frame
    void Update()
    {
        if (numTraversalsLocal != sp.numTraversals | morph != sp.morph | cxt != sp.cxt)
        {
            numTraversalsLocal = sp.numTraversals;

            morph = sp.morph;
            cxt = sp.cxt;
            
            rr.speedBool = 0;

            if (cxt<=.5)
            {
                StartCoroutine(drawSineWall());
            } else
            {
                block_thresh = morph * block_th1 + (1 - morph) * block_th2;
                StartCoroutine(DrawRandBlocks());
            }
            
            
           
        }
    }


    IEnumerator drawSineWall()
    {

        Texture2D texture = new Texture2D(sin_dim1, sin_dim2);
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (var r in renderers)
        {
            // Do something with the renderer here...
            r.material.mainTexture = texture; // like disable it for example. 
        }



        float theta = morph * sin_theta1 + (1 - morph) * sin_theta2;
        float f = morph * sin_f1 + (1 - morph) * sin_f2;
        float thetar = theta * Mathf.PI / 180.0f;
        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                float xs = (float)x / (float)sin_dim2;
                float ys = (float)y / (float)sin_dim1;
                intensity = Mathf.Cos(2.0f * Mathf.PI * f * (xs * (Mathf.Cos(thetar + Mathf.PI / 4.0f) + Mathf.Sin(thetar + Mathf.PI / 4.0f)) + ys * (Mathf.Cos(thetar + Mathf.PI / 4.0f) - Mathf.Sin(thetar + Mathf.PI / 4.0f))));


                color.r = intensity;
                color.g = intensity;
                color.b = intensity;


                color.a = 0.0f; // set alpha to 0 (transparency... reduces glare)
                texture.SetPixel(x, y, color);
            }
        }
        texture.filterMode = FilterMode.Point;
        texture.Apply();
        rr.speedBool = 1;
        yield return null;
    }


    IEnumerator DrawRandBlocks()
    {

        Texture2D texture = new Texture2D(block_dim1, block_dim2);
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


                if (UnityEngine.Random.value <= block_thresh)
                {
                    intensity = 1;

                }
                else
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
        rr.speedBool = 1;
        yield return null;
    }
}