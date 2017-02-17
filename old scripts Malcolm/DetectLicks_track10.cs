using UnityEngine;
using System.Collections;
using System;
using System.IO;
using Uniduino;

public class DetectLicks_track10 : MonoBehaviour {

	public Arduino arduino;
	public int pin = 0;
	public AudioClip errorSound;
	public bool lickForReward = false;
	public bool punish = false;

	private int pinValue;
	private int numLicks = 0;
	private int lickFlag = 0;
	private int rewardFlag = 0;
	private int numRewards = 0;
	private AudioSource source;
	//private Vector3 initialPosition;

	void Start( )
	{
		//initialPosition = new Vector3 (0f, 0.5f, 0f);
		source = GetComponent<AudioSource> ();
		arduino = Arduino.global;
		arduino.Setup(ConfigurePins);
	}

	void ConfigurePins( )
	{
		arduino.pinMode (12, PinMode.OUTPUT);
		arduino.pinMode(pin, PinMode.ANALOG);
		arduino.reportAnalog(pin, 1);
	}

	void Update () 
	{
		// check for keypress to change "lick for reward" setting
		if (Input.GetKeyDown (KeyCode.A)) 
		{
			lickForReward = !lickForReward;
		}

		// reset reward flags
		if (transform.position.z<20)
		{
			rewardFlag = 0;
		}

		// check for licks every frame
		pinValue = arduino.analogRead(pin);

		if (pinValue > 500 & lickFlag == 1) 
		{
			lickFlag = 0;
		}
		if (pinValue<500 & lickFlag==0)
		{
			numLicks += 1;
			lickFlag = 1;
			if (lickForReward)
			{
				if (transform.position.z > 80 & transform.position.z < 120)
				{
					if (rewardFlag == 0)
					{
						StartCoroutine (RewardLong ());
						rewardFlag = 1;
						numRewards += 1;
						Debug.Log (numRewards);
					}
				}
				else if (punish)
				{
					if (transform.position.z > 20 & transform.position.z < 60)
					{
						source.PlayOneShot(errorSound,1F);
						rewardFlag = 1;
					}
					else if (transform.position.z > 140)
					{
						source.PlayOneShot(errorSound,1F);
						rewardFlag = 1;
					}
				}
			}
			// Debug.Log("Licks: " + numLicks);
			var sw = new StreamWriter ("/Users/malcolmc/Desktop/licks.txt", true);
			sw.Write (transform.position.z + "\t" + Time.realtimeSinceStartup + "\n");
			sw.Close ();
		}
	}

	IEnumerator RewardShort ()
	{
		arduino.digitalWrite (12, Arduino.HIGH);
		yield return new WaitForSeconds (0.1f);
		arduino.digitalWrite (12, Arduino.LOW);
	}
	
	IEnumerator RewardLong ()
	{
		arduino.digitalWrite (12, Arduino.HIGH);
		yield return new WaitForSeconds (0.2f);
		arduino.digitalWrite (12, Arduino.LOW);
	}

	void OnApplicationQuit ()
	{
		var sw = new StreamWriter ("/Users/malcolmc/Desktop/rewards.txt", true);
		sw.Write (numRewards);
		sw.Close ();
	}

}
