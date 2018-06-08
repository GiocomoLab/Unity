using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GndColor_Octo : MonoBehaviour
{

    private int dim1 = 1;
    private int dim2 = 1;
    

    private SP_Octo sp;
    private float morph;

    private Color color;
    //private Component playerScript;
    private Renderer gndRenderer;
    

    private GameObject player;
    private GameObject blackCam;
    private GameObject gnd;

    private int numTraversalsLocal = -1;

    // Use this for initialization
    void Start()
    {
        player = GameObject.Find("Player");
        sp = player.GetComponent<SP_Octo>();
        blackCam = GameObject.Find("Black Camera");

        gnd = GameObject.Find("Ground");
        gndRenderer = gnd.GetComponent<Renderer>();
        
        morph = sp.morph;

    }

    // Update is called once per frame
    void Update()
    {
        if (numTraversalsLocal != sp.numTraversals | morph != sp.morph)
        {
            numTraversalsLocal = sp.numTraversals;

            morph = sp.morph;
            StartCoroutine(gndColor());

        }
    }

    IEnumerator gndColor()
    {
        Texture2D texture = new Texture2D(dim1, dim2);

        gndRenderer.material.mainTexture = texture;

        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                float intensity = (1f-morph)*.2f + morph*.8f;
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
