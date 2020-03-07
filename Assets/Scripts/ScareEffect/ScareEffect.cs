using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScareEffect : MonoBehaviour
{
    public PlayerControl player;
    public AudioSource audioSource;

    public AudioClip Scream;
    public SpriteRenderer scaryFace;

    public float duration = 10;

    public void Scare()
    {
        // do the scary stuff!
        audioSource.PlayOneShot(Scream);
        scaryFace.enabled = true;

        Invoke(nameof(StopScaryStuff), 10);
    }

    public void StopScaryStuff()
    {
        scaryFace.enabled = false;
    }
}
