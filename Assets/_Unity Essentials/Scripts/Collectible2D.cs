using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Collectible2D : Collectible
{
    ICollectible.CollectibleType _type = ICollectible.CollectibleType.Cheese2D;
    public new ICollectible.CollectibleType type { get => _type; set => _type = value; }


    private void OnTriggerEnter2D(Collider2D other) {

        if (other.CompareTag("Player"))
        {
            OnCollected?.Invoke(type, collectibleValue);
            ImmediatePickup();
        }


    }


}


