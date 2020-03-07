using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScareEffect : MonoBehaviour
{
    public PlayerControl player;
    public AudioSource audioSource;

    public AudioClip Scream;
    public SpriteRenderer scaryFace;

    public void Scare()
    {
        // do the scary stuff!
        audioSource.PlayOneShot(Scream);
        scaryFace.enabled = true;
    }
}
