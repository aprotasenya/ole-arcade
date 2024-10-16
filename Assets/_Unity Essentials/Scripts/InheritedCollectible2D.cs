using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InheritedCollectible2D : Collectible
{

    private void OnTriggerEnter2D(Collider2D other) {

        if (other.CompareTag("Player"))
        {
            onPickup?.Invoke();
            ImmediatePickup();
        }


    }


}


