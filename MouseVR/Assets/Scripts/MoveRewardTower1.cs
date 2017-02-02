using UnityEngine;
using System.Collections;

public class MoveRewardTower1: MonoBehaviour {

	private bool initialPosition;
	private Vector3 offset;

	void Start () 
	{
		initialPosition = true;
		offset = new Vector3 (0, 0, 100);
	}


	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.R))
		{
			if (initialPosition)
			{
				transform.position = transform.position - offset;
				initialPosition = false;
			}
			else
			{
				transform.position = transform.position + offset;
				initialPosition = true;
			}
		}
	
	}
}
