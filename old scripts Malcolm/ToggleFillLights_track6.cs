using UnityEngine;
using System.Collections;

public class ToggleFillLights_track6 : MonoBehaviour {

	public Light fillLight1;
	public Light fillLight2;

	void Start () {
	
	}

	void Update () {
		if (Input.GetKeyDown (KeyCode.L)) {
			fillLight1.enabled = !fillLight1.enabled;
			fillLight2.enabled = !fillLight2.enabled;
		}
	}
}
