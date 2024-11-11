using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Collectible : MonoBehaviour, IRotating, ICollectible
{
    [SerializeField] Vector3 rotationVector = new Vector3(0f, 0.5f, 0f);
    [SerializeField] GameObject onCollectPrefab;
    [SerializeField] protected int collectibleValue = 1;

    ICollectible.CollectibleType _type = ICollectible.CollectibleType.Star3D;
    public ICollectible.CollectibleType type { get => _type; set => _type = value; }

    public static Action<ICollectible.CollectibleType, int> OnCreated;
    public static Action<ICollectible.CollectibleType, int> OnCollected;


    void Start()
    {
        OnCreated?.Invoke(type, collectibleValue);
    }

    void Update()
    {
        Rotate();
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            OnCollected?.Invoke(type, collectibleValue);
            ImmediatePickup();
        }
        
    }

    protected void ImmediatePickup()
    {
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


