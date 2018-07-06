using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerColor_TwoTower : MonoBehaviour
{

    private Color color;
  
    private SP_TwoTower sp;
    private PC_TwoTower pc;
    private Renderer[] renderers;
    private Texture2D texture;
    //public float jitter;
    private int numTraversalsLocal = -1;
    private float morph = -1;


    // Use this for initialization
    void Start()
    {
        GameObject player = GameObject.Find("Player");
        sp = player.GetComponent<SP_TwoTower>();
        pc = player.GetComponent<PC_TwoTower>();
        

        texture = new Texture2D(1, 1);
        renderers = GetComponentsInChildren<Renderer>();

        
        foreach (var r in renderers)
        {
            // Do something with the renderer here...
            r.material.mainTexture = texture; // like disable it for example. 
        }

       // jitter = .2f * (UnityEngine.Random.value - .5f);
        float tmp_morph = pc.towerJitter + sp.morph;
        color = Color.Lerp(Color.green, Color.blue, tmp_morph);
        
        
        texture.SetPixel(1, 1, color);
        texture.filterMode = FilterMode.Point;
        texture.Apply();
        

    }

    // Update is called once per frame
    void Update()
    {

        if (numTraversalsLocal != sp.numTraversals | morph != sp.morph)
        {
            numTraversalsLocal = sp.numTraversals;

            morph = sp.morph;
            //jitter = .2f * (UnityEngine.Random.value - .5f);
            float tmp_morph = pc.towerJitter + morph;



            color = Color.Lerp(Color.green, Color.blue, tmp_morph);
            StartCoroutine(DrawTowers());

        }
    }



    IEnumerator DrawTowers()
    {
        texture = new Texture2D(1, 1);
        renderers = GetComponentsInChildren<Renderer>();

        foreach (var r in renderers)
        {
            // Do something with the renderer here...
            r.material.mainTexture = texture; // like disable it for example. 
        }




        texture.SetPixel(1, 1, color);
        texture.filterMode = FilterMode.Point;
        texture.Apply();
        yield return null;
    }
       
        
     

    
}
