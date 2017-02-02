using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Linq;

public class OpenLoop : MonoBehaviour {

	// reads in a position file from a previous session and plays it back frame by frame
	
	public float speed = 1;
	public string session = "0828_1";
	private float realSpeed;
	private float originalSpeed;
	private float originalRealSpeed;
	private int delay;
	private bool manipTrial_local = false;
	private PlayerController_track11 playerScript;
	private SessionParams sessionParamsScript;
	private float speedGain;
	private SynchronizeComputers_track5 syncScript;
	private bool recordingStarted_local = false;
	private string[] positionData;
	private string[] thisLine;
	private string mouse = "";
	private string fileName = "";
	private int i = 0;
	private float zpos;

	void Start()
	{

		// connect to playerController script
		GameObject player = GameObject.Find ("Player");
		playerScript = player.GetComponent<PlayerController_track11> ();
		sessionParamsScript = player.GetComponent<SessionParams> ();
		mouse = sessionParamsScript.mouse;


		GameObject gameControl = GameObject.Find ("Game Control");
		syncScript = gameControl.GetComponent<SynchronizeComputers_track5> ();

		fileName = "/Users/malcolmc/Desktop/" + mouse + "/" + session + "_position.txt";
		var sr = new StreamReader (fileName);
		positionData = sr.ReadToEnd ().Split('\n');
		sr.Close();

	}

	void Update() 
	{
		if (syncScript.recordingStarted) {
			thisLine = positionData [i].Split ('\t');
			zpos = 1.0f * float.Parse (thisLine [0]);
			Vector3 pos = new Vector3 (0.0f, 0.0f, zpos);
			transform.position = pos;
			i = i + 1;
		}
	}

}
