using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomBlockPattern_TwoTower : MonoBehaviour
{

    private int dim1 = 10;
    private int dim2 = 5;
    private int nPixels;

    private Color color;

    private SP_TwoTower sp;
    private RR_TwoTower rr;
    private GameObject player;

    private GameObject eWall;
    private GameObject wWall;
    private Renderer eastRenderer;
    private Renderer westRenderer;
    private float intensity;

    private int numTraversalsLocal = -1;
    private float morph = -1;
    private float th1 = .1f;
    private float th2 = .9f;
    private float thresh;
    // Use this for initialization
    void Start()
    {
        player = GameObject.Find("Player");
        sp = player.GetComponent<SP_TwoTower>();
        rr = player.GetComponent<RR_TwoTower>();

        nPixels = dim1 * dim2;

        StartCoroutine(DrawRandBlocks());



    }

    // Update is called once per frame
    void Update()
    {
        if (numTraversalsLocal != sp.numTraversals | morph != sp.morph)
        {
            numTraversalsLocal = sp.numTraversals;

            morph = sp.morph;

            rr.speedBool = 0;
            thresh = morph * th1 + (1 - morph) * th2;
            StartCoroutine(DrawRandBlocks());


        }
    }

    IEnumerator DrawRandBlocks()
    {

        //Texture2D texture = new Texture2D(dim1, dim2);
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        float[] pixelVals;
        pixelVals = new float[nPixels];

        for (int i = 0; i < nPixels; i++)
        {
            if (i < (nPixels * thresh))
            {

                pixelVals[i] = 0f;
            }
            else
            {
                pixelVals[i] = 1f;
            }
        }

        float[] pixelValsScram = FisherYates(pixelVals);



        foreach (var r in renderers)
        {
            // Do something with the renderer here...
            pixelValsScram = FisherYates(pixelValsScram);
            Texture2D texture = new Texture2D(dim1, dim2);
            r.material.mainTexture = texture; // like disable it for example. 
            for (int y = 0; y < texture.height; y++)
            {
                for (int x = 0; x < texture.width; x++)
                {
                    //Debug.Log(y * dim2 + x);
                    intensity = pixelValsScram[y * dim2 + x];
                    color.r = intensity;
                    color.g = intensity;
                    color.b = intensity;


                    color.a = 0.0f; // set alpha to 0 (transparency... reduces glare)
                    texture.SetPixel(x, y, color);
                }
            }
            texture.filterMode = FilterMode.Point;
            texture.Apply();


        }



       
        rr.speedBool = 1;
        yield return null;
    }

    float[] FisherYates(float[] origArray)
    {
        // then shuffle values (Fisher-Yates shuffle)
        int[] order = new int[origArray.Length];

        for (int i = 0; i < origArray.Length; i++)
        {
            order[i] = i;
        }


        for (int i = order.Length - 1; i >= 0; i--)
        {
            int r = (int)UnityEngine.Mathf.Round(UnityEngine.Random.Range(0, i));
            int tmp = order[i];
            order[i] = order[r];
            order[r] = tmp;
        }

        float[] permArray = new float[origArray.Length];
        for (int i = 0; i < origArray.Length; i++)
        {
            permArray[i] = origArray[order[i]];
        }

        return permArray;
    }
}
