using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetGroundColor : MonoBehaviour {

    private Color color;
    
    private SP  sp;
    private float morph;
    private GameObject player;
    private Renderer[] renderers;
    private int numTraversalsLocal = -1;
    

    // Use this for initialization
    void Start () {
        player = GameObject.Find("Player");
        sp = player.GetComponent<SP>();
        morph = sp.morph;

        Texture2D texture = new Texture2D(1, 1);
        renderers = GetComponents<Renderer>();

        //float i = Random.Range(0.0f, 1.0f);
        foreach (var r in renderers)
        {
            // Do something with the renderer here...
            r.material.mainTexture = texture; // like disable it for example. 
        }

        //if (staticColor) {
            //color.r = red; color.g = green; color.b = blue; color.a = 0.0f;
        //} else {
            color.r = morph; color.g = morph; color.b = morph;
            color.a = 0.0f;
        //}
        texture.SetPixel(1, 1, color);
        texture.filterMode = FilterMode.Point;
        texture.Apply();

    }

    // Update is called once per frame
    void Update()
    {
        Texture2D texture = new Texture2D(1, 1);


        if (player.transform.position.z < 0 & numTraversalsLocal != sp.numTraversals)
        {
            //float i = Random.Range(0.0f, 1.0f);
            foreach (var r in renderers)
            {
                // Do something with the renderer here...
                r.material.mainTexture = texture; // like disable it for example. 
            }

            //if (staticColor) {
            //color.r = red; color.g = green; color.b = blue; color.a = 0.0f;
            //} else {
            color.r = morph; color.g = morph; color.b = morph;
            color.a = 0.0f;
            //}
            texture.SetPixel(1, 1, color);
            texture.filterMode = FilterMode.Point;
            texture.Apply();
        }
    }
}
