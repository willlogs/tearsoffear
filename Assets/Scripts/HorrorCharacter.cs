using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorrorCharacter : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip facingSound;
    public Light leftLight, rightLight, faceLight;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            audioSource.PlayOneShot(facingSound);
            leftLight.enabled = true;
            rightLight.enabled = true;
            faceLight.enabled = true;
        }
    }
}