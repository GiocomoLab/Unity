using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeAudioSourcePitch : MonoBehaviour {
    private SessionParams paramsScript;
    private float morph;
    private AudioSource sound;
    
    // Use this for initialization
    void Start () {

        GameObject player = GameObject.Find("Player");
        paramsScript = player.GetComponent<SessionParams>();
        morph = paramsScript.morph;


        sound = GetComponent<AudioSource>();
        sound.pitch = 3.0f - 2.0f * morph;
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
