using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Collectible : MonoBehaviour, IRotating, ICollectible
{
    [SerializeField] Vector3 rotationVector = new Vector3(0f, 0.5f, 0f);
    [SerializeField] GameObject onCollectPrefab;
    [SerializeField] protected int collectibleValue = 1;

    [SerializeField] CollectibleType _type = CollectibleType.Star3D;
    public CollectibleType Type { get => _type; set => _type = value; }

    public static event Action<CollectibleType, int> OnCreated;
    public static event Action<CollectibleType, int> OnCollected;


    void Start()
    {
        OnCreated?.Invoke(Type, collectibleValue);
    }

    void Update()
    {
        Rotate();
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            ImmediatePickup();
        }
        
    }

    protected void ImmediatePickup()
    {
        OnCollected?.Invoke(Type, collectibleValue);
        Instantiate(onCollectPrefab, transform.position, transform.rotation);
        Destroy(gameObject);
    }

    public void Rotate()
    {
        transform.Rotate(rotationVector);
    }
}

public interface IRotating
{
    public abstract void Rotate();
}


