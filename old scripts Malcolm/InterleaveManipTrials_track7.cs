using UnityEngine;
using System.Collections;
using System.IO;

public class InterleaveManipTrials_track7 : MonoBehaviour {

	public float manipProbability = 0;
	private float posz = 0;
	private int randNum = 0;
	private bool trialType = false;

	void Start () {
	
	}

	void Update () {
		if (transform.position.z - posz < -300) 
		{
			randNum = Random.Range(0,1000);
			trialType = randNum > 1000 * manipProbability;
			var sw = new StreamWriter ("/Users/malcolmc/Desktop/trial_type.txt", true);
			sw.Write (Time.realtimeSinceStartup + "\t" + trialType + "\n");
			sw.Close ();
			Debug.Log ("Manip: " + trialType);
		}
		posz = transform.position.z;
	}
}
