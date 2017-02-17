using UnityEngine;
using System.Collections;

public class ToggleTowers_track7 : MonoBehaviour {

	private bool towersOn;
	public GameObject tower1;
	public GameObject tower2;
	public GameObject tower3;
	public GameObject tower4;
	public GameObject tower5;
	public GameObject tower6;
	public GameObject tower7;
	public GameObject tower8;
	public GameObject tower9;
	public GameObject tower10;
	private bool manipTrial_local = false;
	private PlayerController_track7 playerScript;

	void Start () {
		towersOn = true;
		GameObject player = GameObject.Find ("Player");
		playerScript = player.GetComponent<PlayerController_track7> ();
	}

	void Update () {
		if (Input.GetKeyDown (KeyCode.T))
		{
			tower1.SetActive(!towersOn);
			tower2.SetActive(!towersOn);
			tower3.SetActive(!towersOn);
			tower4.SetActive(!towersOn);
			tower5.SetActive(!towersOn);
			tower6.SetActive(!towersOn);
			tower7.SetActive(!towersOn);
			tower8.SetActive(!towersOn);
			tower9.SetActive(!towersOn);
			tower10.SetActive(!towersOn);
			towersOn = !towersOn;
			Debug.Log("Towers: " + towersOn);
		}

		if (playerScript.zenTrack)
		{
			if(playerScript.manipTrial != manipTrial_local)
			{
				tower1.SetActive(!towersOn);
				tower2.SetActive(!towersOn);
				tower3.SetActive(!towersOn);
				tower4.SetActive(!towersOn);
				tower5.SetActive(!towersOn);
				tower6.SetActive(!towersOn);
				tower7.SetActive(!towersOn);
				tower8.SetActive(!towersOn);
				tower9.SetActive(!towersOn);
				tower10.SetActive(!towersOn);
				towersOn = !towersOn;
				manipTrial_local = playerScript.manipTrial;
			}
		}
	}
}
