using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootStepSFX : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] sfx;
    public bool walking = false;
    public float betweenSteps = 0.5f;

    float time = 0;

    public void Walk()
    {
        CancelInvoke(nameof(StopWalking));
        walking = true;
        Invoke(nameof(StopWalking), betweenSteps/2);
    }

    private void StopWalking()
    {
        walking = false;
    }

    private void Update()
    {
        if (sfx.Length > 0)
        {
            if (walking || time < betweenSteps)
                time += Time.deltaTime;

            if (time > betweenSteps && walking)
            {
                time = 0;
                PlaySFX();
            }
        }
    }

    private void PlaySFX()
    {
        int indx = Random.Range(0, sfx.Length);

        audioSource.PlayOneShot(sfx[indx]);
    }
}
