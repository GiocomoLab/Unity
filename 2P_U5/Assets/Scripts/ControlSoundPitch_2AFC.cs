using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlSoundPitch_2AFC : MonoBehaviour {

    private AudioSource sound;
    private float pitch1 = 1;
    private float pitch2 = 3;

    private SessionParams_2AFC sessionParams;
    private GameObject player;
    private float morph;

    // Use this for initialization
    void Start () {
        player = GameObject.Find("Player");
        sessionParams = player.GetComponent<SessionParams_2AFC>();
        sound = GameObject.Find("basic_maze").GetComponent<AudioSource>();
        morph = sessionParams.morph;

        StartCoroutine(ChangePitch());
	}
	
	// Update is called once per frame
	void Update () {

        if (player.transform.position.z<=0.0f & morph!=sessionParams.morph )
        {
            morph = sessionParams.morph;
            StartCoroutine(ChangePitch());
        }
		
	}

    IEnumerator ChangePitch()
    {
        sound.pitch = morph * pitch1 + (1.0f - morph) * pitch2;

        yield return null;
    }
}
