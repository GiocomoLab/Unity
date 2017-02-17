using UnityEngine;
using System.Collections;

public class ChangeMaterial : MonoBehaviour {

	public Material material1;
	public Material material2;
	public Renderer rend;
	private bool flag = true;
	
	void Start () {
		rend = GetComponent<Renderer> ();
		rend.enabled = true;
	}

	void Update () {
		if (Input.GetKeyDown(KeyCode.M)) 
		{
			if (flag)
			{
				rend.material = material2;
				flag = false;
			}
			else
			{
				rend.material = material1;
				flag = true;
			}
		}
	}
}
