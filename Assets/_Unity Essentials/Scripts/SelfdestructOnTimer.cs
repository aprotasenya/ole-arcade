using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfdestructOnTimer : MonoBehaviour
{
    [SerializeField] float selfdestructAfter = 1f;
    float currentTime = 0;

    void Start()
    {
        currentTime = selfdestructAfter;
    }

    void Update()
    {
        currentTime -= Time.deltaTime;
        if (currentTime <= 0) SelfDesctruct();
    }

    private void SelfDesctruct()
    {
        Destroy(gameObject);
    }
}
