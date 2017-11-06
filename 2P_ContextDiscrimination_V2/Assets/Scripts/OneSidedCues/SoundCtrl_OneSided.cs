using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundCtrl_OneSided : MonoBehaviour
{

    private AudioSource sound;
    private float pitch1 = 1;
    private float pitch2 = 3;

    private SP_OneSided sp;
    private GameObject player;
    private float morph;

    // Use this for initialization
    void Start()
    {
        player = GameObject.Find("Player");
        sp = player.GetComponent<SP_OneSided>();
        sound = player.GetComponent<AudioSource>();
        morph = sp.morph;

        StartCoroutine(ChangePitch());
    }

    // Update is called once per frame
    void Update()
    {

        if (player.transform.position.z <= 0.0f & morph != sp.morph)
        {
            morph = sp.morph;
            StartCoroutine(ChangePitch());
        }

    }

    IEnumerator ChangePitch()
    {
        sound.pitch = morph * pitch1 + (1.0f - morph) * pitch2;

        yield return null;
    }
}
