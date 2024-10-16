using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Collectible : MonoBehaviour
{
    [SerializeField] Vector3 rotationVector = new Vector3(0,0.5f,0);
    [SerializeField] GameObject onCollectPrefab;

    public static Action onCreate;
    public static Action onPickup;


    void Start()
    {
        onCreate?.Invoke();
    }

    void Update()
    {
        transform.Rotate(rotationVector);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            onPickup?.Invoke();
            ImmediatePickup();
        }
        
    }

    void ImmediatePickup()
    {
        Instantiate(onCollectPrefab, transform.position, transform.rotation);
        Destroy(gameObject);
    }
 
}
