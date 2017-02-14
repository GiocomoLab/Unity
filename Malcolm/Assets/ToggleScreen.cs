using UnityEngine;
using System.Collections;

public class ToggleScreen : MonoBehaviour {

	public GUITexture guiTextre;
		
	void Awake ()
	{
		// Set the texture so that it is the the size of the screen and covers it.
		guiTextre.pixelInset = new Rect(0f, 0f, Screen.width, Screen.height);
		guiTextre.enabled = false;
	}
		
	void Update ()
	{
		if(Input.GetKeyDown(KeyCode.F1))
		{	
			StartCoroutine(ToggleTexture());
		}
	}

	IEnumerator ToggleTexture ()
	{
		guiTextre.enabled = ! guiTextre.enabled;
		yield return null;
	}

}
