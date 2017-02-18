using UnityEngine;
using System.Collections;

public class ToggleTowers : MonoBehaviour {

	private bool towersOn;
	public GameObject tower1;
	public GameObject tower2;
	public GameObject tower3;
	public GameObject tower4;
	public GameObject tower5;
	public GameObject tower6;
	public GameObject tower7;
	public GameObject tower8;


	void Start () {
		towersOn = true;
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
			towersOn = !towersOn;
			Debug.Log("Towers: " + towersOn);
		}
	}
}
