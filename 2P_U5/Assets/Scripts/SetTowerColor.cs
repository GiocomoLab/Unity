using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTowerColor : MonoBehaviour {

    private Color color;

	// Use this for initialization
	void Start () {
        Texture2D texture = new Texture2D(1, 1);
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        float i = Random.Range(0.0f, 1.0f);
        foreach (var r in renderers)
        {
            // Do something with the renderer here...
            r.material.mainTexture = texture; // like disable it for example. 
        }
        color.r = i; color.g = i; color.b = i;
        color.a = 0.0f;
        texture.SetPixel(1, 1, color);
        texture.filterMode = FilterMode.Point;
        texture.Apply();

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
