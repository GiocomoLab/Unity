using UnityEngine;
using System.Collections;

public class ToggleWalls_track12 : MonoBehaviour {

	private bool wallsNormal;
	public GameObject eastWall1;
	public GameObject westWall1;
	public Material material1;
	public Material manipMaterial;
	private bool manipTrial_local = false;
	private PlayerController_track11 playerScript;

	void Start () {
		wallsNormal = true;
		GameObject player = GameObject.Find ("Player");
		playerScript = player.GetComponent<PlayerController_track11> ();
	}

	void Update () {
		if (playerScript.flipStripes)
		{
			if(playerScript.manipTrial != manipTrial_local)
			{
				if (wallsNormal)
				{
					eastWall1.GetComponent<Renderer>().material = manipMaterial;
					westWall1.GetComponent<Renderer>().material = manipMaterial;
				}
				else 
				{
					eastWall1.GetComponent<Renderer>().material = material1;
					westWall1.GetComponent<Renderer>().material = material1;
				}
				wallsNormal = !wallsNormal;
				manipTrial_local = playerScript.manipTrial;
			}
		}
	}
}
