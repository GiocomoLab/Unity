using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.IO.Ports;
using System.Threading;

public class MoveAtConstantSpeed : MonoBehaviour {
	
	public float speed = 1;
	private float realSpeed;
	private float originalSpeed;
	private float originalRealSpeed;
	private int delay;
	private bool manipTrial_local = false;
	private PlayerController_track11_old playerScript;
	private float speedGain;
	private SynchronizeComputers_track5 syncScript;
	private bool recordingStarted_local = false;

	void Start()
	{

		// set speed
		realSpeed = 0;

		// connect to playerController script
		GameObject player = GameObject.Find ("Player");
		playerScript = player.GetComponent<PlayerController_track11_old> ();
		speedGain = playerScript.speedGain;

		GameObject gameControl = GameObject.Find ("Game Control");
		syncScript = gameControl.GetComponent<SynchronizeComputers_track5> ();
	}

	void Update() 
	{
		if (syncScript.recordingStarted & !recordingStarted_local) 
		{
			realSpeed = speed * 0.0614f * (429f/400f);	
			originalRealSpeed = realSpeed;
			originalSpeed = 1;
			recordingStarted_local = true;
		}

		if (Input.GetKeyDown (KeyCode.UpArrow)) 
		{
			speed = speed*2;
			realSpeed = realSpeed*2;
		}
		if (Input.GetKeyDown (KeyCode.DownArrow)) 
		{
			speed = speed/2;
			realSpeed = realSpeed/2;	
		}

		float delta_z = 0.0f;
		if (transform.position.z<20 | transform.position.z>425)
		{
			delta_z = 0.1f * realSpeed;
		} else { 
			delta_z = 1.0f * realSpeed;
		}

		Vector3 movement = new Vector3 (0.0f, 0.0f, delta_z);
		transform.position = transform.position+movement;

		// change speed if manip trial
		if (playerScript.manipSession)
		{
			if(playerScript.manipTrial & !manipTrial_local)
			{
				speedGain = playerScript.speedGain;
				realSpeed = realSpeed * speedGain;
				speed = speed * speedGain;
				manipTrial_local = playerScript.manipTrial;
			}
			else if(!playerScript.manipTrial & manipTrial_local)
			{
				realSpeed = realSpeed / speedGain;
				speed = speed / speedGain;
				manipTrial_local = playerScript.manipTrial;
			}
		} else if (playerScript.testRangeOfGains) {
			speedGain = playerScript.speedGain;
			realSpeed = originalRealSpeed * speedGain;
			speed = speedGain;
			// Debug.Log (speed);
			// manipTrial_local = playerScript.manipTrial;
		}
	}

}
