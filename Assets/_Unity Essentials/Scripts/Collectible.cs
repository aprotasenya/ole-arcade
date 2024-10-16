using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    [SerializeField] Vector3 rotationVector = new Vector3(0,0.5f,0);
    [SerializeField] GameObject onCollectPrefab;


    void Start()
    {
    }

    void Update()
    {
        transform.Rotate(rotationVector);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) ImmediatePickup();
        
    }

    void ImmediatePickup()
    {
        Instantiate(onCollectPrefab, transform.position, transform.rotation);
        Destroy(gameObject);
    }
 
}
