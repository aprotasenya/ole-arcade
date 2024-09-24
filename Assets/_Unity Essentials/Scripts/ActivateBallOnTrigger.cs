using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateBallOnTrigger : MonoBehaviour
{
    [SerializeField] private Rigidbody ball;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            ball.useGravity = true;
        }
    }

}
