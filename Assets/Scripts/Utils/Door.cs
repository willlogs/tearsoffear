using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Door : Interactive
{
    public float rot = 90;
    public int index = -1;

    public AudioClip[] squeek;
    public AudioSource source;

    public bool open = false;

    bool moving;
    Vector3 baseRot;
    public Vector3 goalRot;

    DoorManager dm;

    private void Start()
    {
        baseRot = transform.rotation.eulerAngles;
        source = GetComponent<AudioSource>();
        dm = FindObjectOfType<DoorManager>();
    }

    public void OpenUp()
    {
        transform.DORotate(transform.rotation.eulerAngles + new Vector3(0, 0, -rot), 0.5f);
        open = true;
    }

    public void Close()
    {
        transform.DORotate(baseRot, 0.5f);
        open = false;
    }

    public void ToggleState()
    {
        if (open) Close();
        else OpenUp();

        int r = Random.Range(0, squeek.Length);
        source.Stop();
        source.PlayOneShot(squeek[r]);
    }

    public override void Interact()
    {
        dm.Toggled(index);
        ToggleState();
    }

    public override void GhostInteract()
    {
        if (!open)
        {
            ToggleState();
            dm.Toggled(index);
        }
    }
}
