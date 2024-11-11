using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Collectible2D : Collectible
{
    //public CollectibleType _type = CollectibleType.Cheese2D;
    //public new CollectibleType Type { get => _type; set => _type = value; }


    private void OnTriggerEnter2D(Collider2D other) {

        if (other.CompareTag("Player"))
        {
            ImmediatePickup();
        }


    }


}


