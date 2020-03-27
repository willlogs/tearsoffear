using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Door : Interactive
{
    // 0 -> closed
    // 1 -> opened

    public float rot = 90;
    public int indexx = -1;

    public AudioClip[] squeek;
    public AudioSource source;

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
        state = 1;
    }

    public void Close()
    {
        transform.DORotate(baseRot, 0.5f);
        state = 0;
    }

    public void ToggleState()
    {
        print("toggling door");
        if (state == 1) Close();
        else OpenUp();

        int r = Random.Range(0, squeek.Length);
        source.Stop();
        source.PlayOneShot(squeek[r]);
        print("door toggled");
    }

    public override void Interact()
    {
        dm.Toggled(gameObject.name);
        ToggleState();
    }

    public override void GhostInteract()
    {
        if (state != 1)
        {
            ToggleState();
            dm.Toggled(gameObject.name);
        }
    }

    protected override void ApplyState()
    {
        if(nextState == 0)
        {
            Close();
        }
        else
        {
            OpenUp();
        }
    }
}
