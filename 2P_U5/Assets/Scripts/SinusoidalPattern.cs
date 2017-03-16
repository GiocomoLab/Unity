using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinusoidalPattern : MonoBehaviour {

    public int dim1 = 700;
    public int dim2 = 1;
    public float f = 1;
    public float phase;
    private Color color;
    private int numTraversals_local = 0;
    private PlayerController playerScript;
    private Renderer eastRenderer;
    private Renderer westRenderer;

    // Use this for initialization
    void Start () {
        // find walls
        GameObject EastWall = GameObject.Find("East Wall");
        eastRenderer = EastWall.GetComponent<Renderer>();
        GameObject WestWall = GameObject.Find("West Wall");
        westRenderer = WestWall.GetComponent<Renderer>();

        // add texture to object
        StartCoroutine(drawSineWall());

        // find player
        GameObject player = GameObject.Find("Player");
        playerScript = player.GetComponent<PlayerController>();

        

    }
	
	// Update is called once per frame
	void Update () {
        if (playerScript.numTraversals > numTraversals_local)
        {
            numTraversals_local = playerScript.numTraversals;
            StartCoroutine(drawSineWall());
        }

    }

    IEnumerator drawSineWall()
    {

        Texture2D texture = new Texture2D(dim1, dim2);
  
        eastRenderer.material.mainTexture = texture;
        westRenderer.material.mainTexture = texture;

        float phase = 2 * Mathf.PI * Random.Range(0.0f, 1.0f);

        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                
                float intensity = 0.5f * (Mathf.Sin(2 * Mathf.PI * (float)x *f / (float)dim1 + phase) + 1.0f) ;
                color.r = intensity;
                color.g = intensity;
                color.b = intensity;

                //if (UnityEngine.Random.Range(0, 2) == 1)
                //{
                //    color = Color.black;
                //}
                //else
                //{
                //   color = Color.white;
                //}
                color.a = 0.0f; // set alpha to 0 (transparency... reduces glare)
                texture.SetPixel(x, y, color);
            }
        }
        texture.filterMode = FilterMode.Point;
        texture.Apply();

        yield return null;
    }
}
