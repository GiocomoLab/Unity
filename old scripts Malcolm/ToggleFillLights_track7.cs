using UnityEngine;
using System.Collections;

public class ToggleFillLights_track7 : MonoBehaviour {

	public Light fillLight1;
	public Light fillLight2;
	public Light dimFillLight1;
	public Light dimFillLight2;
	private bool manipTrial_local = false;
	private PlayerController_track7 playerScript;

	void Start () {
		GameObject player = GameObject.Find ("Player");
		playerScript = player.GetComponent<PlayerController_track7> ();
	}

	void Update () {
		if (Input.GetKeyDown (KeyCode.L)) {
			fillLight1.enabled = !fillLight1.enabled;
			fillLight2.enabled = !fillLight2.enabled;
			dimFillLight1.enabled = !dimFillLight1.enabled;
			dimFillLight2.enabled = !dimFillLight2.enabled;
		}

		if (playerScript.zenTrack)
		{
			if(playerScript.manipTrial != manipTrial_local)
			{
				fillLight1.enabled = !fillLight1.enabled;
				fillLight2.enabled = !fillLight2.enabled;
				dimFillLight1.enabled = !dimFillLight1.enabled;
				dimFillLight2.enabled = !dimFillLight2.enabled;
				manipTrial_local = playerScript.manipTrial;
			}
		}
	}
}
