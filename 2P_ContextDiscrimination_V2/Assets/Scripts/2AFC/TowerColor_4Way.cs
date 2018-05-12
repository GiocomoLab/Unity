using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerColor_4Way: MonoBehaviour
{

    private int dim1 = 100;
    private int dim2 = 3000;
    private float f1 = .8f;
    private float f2 = .1f;
    private float theta = 90f;

    private SP_4Way sp;
    private float morph;
    private float cxt;
    private Color color;
    private Renderer eastRenderer;
    private Renderer westRenderer;
    private Texture2D texture;
    private float phase;

    private GameObject player;



    private RR_4Way rr;
    //  private TrialOrdering_Test trialOrder;

    private int numTraversalsLocal = -1;




    // Use this for initialization
    void Start()
    {
        player = GameObject.Find("Player");
        sp = player.GetComponent<SP_4Way>();

        rr = player.GetComponent<RR_4Way>();
        //       trialOrder = player.GetComponent<TrialOrdering_Test>();




        morph = sp.morph;
        cxt = sp.cxt;
        phase = 2.0f * Mathf.PI * UnityEngine.Random.value;

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
            phase = UnityEngine.Random.value;

            if (cxt > .5f)
            {
                StartCoroutine(drawSineWall());
            } else
            {
                texture = new Texture2D(1, 1);
                Renderer[] renderers = GetComponentsInChildren<Renderer>();
                renderers = GetComponentsInChildren<Renderer>();

                foreach (var r in renderers)
                {
                    // Do something with the renderer here...
                    r.material.mainTexture = texture; // like disable it for example. 
                }

                color = Color.Lerp(Color.green, Color.blue, sp.morph);


                texture.SetPixel(1, 1, color);
                texture.filterMode = FilterMode.Point;
                texture.Apply();
                rr.speedBool = 1;
            }


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




        float f = morph * f1 + (1 - morph) * f2;
        float thetar = theta * Mathf.PI / 180.0f;
        for (int y = 0; y <= texture.height; y++)
        {
            for (int x = 0; x <= texture.width; x++)
            {
                float xs = (float)x / (float)dim2;
                float ys = (float)y / (float)dim1;
                float intensity = Mathf.Cos(2.0f * Mathf.PI * f * ((xs - phase) * (Mathf.Cos(thetar + Mathf.PI / 4.0f) + Mathf.Sin(thetar + Mathf.PI / 4.0f)) + (ys - phase) * (Mathf.Cos(thetar + Mathf.PI / 4.0f) - Mathf.Sin(thetar + Mathf.PI / 4.0f))));


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

