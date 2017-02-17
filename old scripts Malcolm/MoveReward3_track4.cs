using UnityEngine;
using System.Collections;

public class MoveReward3_track4: MonoBehaviour {

	private bool isInInitialPosition;
	private Vector3 initialPosition;
	private Vector3 alternatePosition;
	
	void Start () 
	{
		isInInitialPosition = true;
		initialPosition = transform.position;
		alternatePosition = new Vector3 (0, 0, 335);
	}
	
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.R))
		{
			if (isInInitialPosition)
			{
				transform.position = alternatePosition;
				isInInitialPosition = false;
			}
			else
			{
				transform.position = initialPosition;
				isInInitialPosition = true;
			}
		}
		
	}
}
