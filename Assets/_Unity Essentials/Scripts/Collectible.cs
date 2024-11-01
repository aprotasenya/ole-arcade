using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Collectible : MonoBehaviour, IRotating
{
    [SerializeField] Vector3 rotationVector = new Vector3(0f, 0.5f, 0f);
    [SerializeField] GameObject onCollectPrefab;
    [SerializeField] protected int collectibleValue = 1;

    public static Action<int> OnCreated;
    public static Action<int> OnCollected;


    void Start()
    {
        OnCreated?.Invoke(collectibleValue);
    }

    void Update()
    {
        Rotate();
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            OnCollected?.Invoke(collectibleValue);
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


