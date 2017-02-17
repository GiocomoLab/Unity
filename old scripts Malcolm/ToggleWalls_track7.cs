using UnityEngine;
using System.Collections;

public class ToggleWalls_track7 : MonoBehaviour {

	private bool wallsNormal;
	public GameObject eastWall1;
	public GameObject eastWall2;
	public GameObject eastWall3;
	public GameObject westWall1;
	public GameObject westWall2;
	public GameObject westWall3;
	public Material material1;
	public Material material2;
	public Material material3;
	public Material manipMaterial;
	private bool manipTrial_local = false;
	private PlayerController_track7 playerScript;

	void Start () {
		wallsNormal = true;
		GameObject player = GameObject.Find ("Player");
		playerScript = player.GetComponent<PlayerController_track7> ();
	}

	void Update () {
		if (playerScript.zenTrack)
		{
			if(playerScript.manipTrial != manipTrial_local)
			{
				if (wallsNormal)
				{
					eastWall1.GetComponent<Renderer>().material = manipMaterial;
					eastWall2.GetComponent<Renderer>().material = manipMaterial;
					eastWall3.GetComponent<Renderer>().material = manipMaterial;
					westWall1.GetComponent<Renderer>().material = manipMaterial;
					westWall2.GetComponent<Renderer>().material = manipMaterial;
					westWall3.GetComponent<Renderer>().material = manipMaterial;
				}
				else 
				{
					eastWall1.GetComponent<Renderer>().material = material1;
					eastWall2.GetComponent<Renderer>().material = material2;
					eastWall3.GetComponent<Renderer>().material = material3;
					westWall1.GetComponent<Renderer>().material = material1;
					westWall2.GetComponent<Renderer>().material = material2;
					westWall3.GetComponent<Renderer>().material = material3;
				}
				wallsNormal = !wallsNormal;
				manipTrial_local = playerScript.manipTrial;
			}
		}
	}
}
