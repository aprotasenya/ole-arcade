using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible2D : MonoBehaviour
{

    public float rotationSpeed = 0.5f;
    public GameObject onCollectEffect;

    // Update is called once per frame
    void Update()
    {

        transform.Rotate(0, 0, rotationSpeed);
        
    }

    private void OnTriggerEnter2D(Collider2D other) {
        
        if (other.GetComponent<PlayerController2D>() != null) {
            Instantiate(onCollectEffect, transform.position, transform.rotation);
            Destroy(gameObject);

        }

        
    }


}


