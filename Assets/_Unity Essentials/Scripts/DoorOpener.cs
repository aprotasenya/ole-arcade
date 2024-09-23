using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DoorOpener : MonoBehaviour
{
    [SerializeField] private Animator doorAnimator;

    void Start()
    {

    }

    private void SetDoorDirection(Collider other)
    {
        Vector3 relativePos = transform.InverseTransformPoint(other.transform.position);
        bool openToTheLeft = relativePos.z < 0;
        doorAnimator.SetBool("openToTheLeft", openToTheLeft);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || doorAnimator == null) { return; }

        // Trigger the Door_Open animation
        SetDoorDirection(other);
        doorAnimator.SetTrigger("Door_Open");
    }


    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player") || doorAnimator == null) { return; }

        // Trigger the Door_Open animation
        doorAnimator.SetTrigger("Door_Close");
    }
}