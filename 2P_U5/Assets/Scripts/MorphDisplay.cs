using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MorphDisplay : MonoBehaviour
{
	float deltaTime = 0.0f;
    private SessionParams_2AFC paramsScript;
    private float morph;
    private GameObject player;

    void Start()
    {
        player = GameObject.Find("Player");
        paramsScript = player.GetComponent<SessionParams_2AFC>();
        morph = paramsScript.morph;
    }
    void Update()
	{
		deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        morph = paramsScript.morph;
    }

	void OnGUI()
	{
		int w = Screen.width, h = Screen.height;

		GUIStyle style = new GUIStyle();

		Rect rect = new Rect(0, 0, w, h * 2 / 100);
		style.alignment = TextAnchor.UpperLeft;
		style.fontSize = h * 2 / 100;
		style.normal.textColor = new Color (0.0f, 0.0f, 0.5f, 1.0f);
		
       
		string text = string.Format(" {0}", morph);
		GUI.Label(rect, text, style);
	}
}

