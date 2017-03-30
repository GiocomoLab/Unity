using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class GenerateBlockPattern : MonoBehaviour
{

    public int dim1 = 300;
    public int dim2 = 3;
    private Color color;
    private int numTraversals_local = 0;
    private PlayerController playerScript;
    private GameObject EastWall;
    private GameObject WestWall;

    void Start()
    {

        // add texture to object
        StartCoroutine(ChangeBlockPattern());

        // find player
        GameObject player = GameObject.Find("Player");
        playerScript = player.GetComponent<PlayerController>();

        EastWall = GameObject.Find("East Wall");
        WestWall = GameObject.Find("West Wall");
    }

    void Update()
    {
        if (playerScript.numTraversals > numTraversals_local)
        {
            numTraversals_local = playerScript.numTraversals;
            StartCoroutine(ChangeBlockPattern());
        }
    }

    IEnumerator ChangeBlockPattern()
    {

        Texture2D texture = new Texture2D(dim1, dim2);
        EastWall.GetComponent<Renderer>().material.mainTexture = texture;
        WestWall.GetComponent<Renderer>().material.mainTexture = texture;

        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                if (UnityEngine.Random.Range(0, 2) == 1)
                {
                    color = Color.black;
                }
                else
                {
                    color = Color.white;
                }
                color.a = 0.0f; // set alpha to 0 (transparency... reduces glare)
                texture.SetPixel(x, y, color);
            }
        }
        texture.filterMode = FilterMode.Point;
        texture.Apply();

        yield return null;
    }
}
