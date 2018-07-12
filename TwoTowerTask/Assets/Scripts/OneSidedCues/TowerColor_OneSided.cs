using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerColor_OneSided : MonoBehaviour
{

    private Color color;
    private SP_OneSided sp;
    private Renderer[] renderers;
    private Texture2D texture;
    


    // Use this for initialization
    void Start()
    {
        GameObject player = GameObject.Find("Player");
        sp = player.GetComponent<SP_OneSided>();
        

        texture = new Texture2D(1, 1);
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

    }

    // Update is called once per frame
    void Update()
    {
        texture = new Texture2D(1, 1);
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

    }
}
