// 9/19/2024 AI-Tag
// This was created with assistance from Muse, a Unity Artificial Intelligence product

using System;
using UnityEditor;
using UnityEngine;

public class DayCycle : MonoBehaviour
{
    public Light directionalLight;
    public float cycleDuration = 120.0f; // Duration of the day cycle in seconds
    public Color dayColor = Color.white;
    public Color nightColor = Color.blue;

    private float cycleTimer;

    private void Start()
    {
        directionalLight = GetComponent<Light>();
    }

    void Update()
    {
        // Update the cycle timer
        cycleTimer += Time.deltaTime;

        // Calculate the cycle progress
        float cycleProgress = cycleTimer / cycleDuration;

        // Rotate the light
        float rotationAngle = cycleProgress * 360.0f; // Complete a full rotation (360 degrees) over the cycle
        directionalLight.transform.rotation = Quaternion.Euler(rotationAngle, -30.0f, 0);

        // Change the light color
        directionalLight.color = Color.Lerp(nightColor, dayColor, Mathf.Sin(cycleProgress * Mathf.PI));

        // Reset cycle timer
        if (cycleTimer >= cycleDuration)
            cycleTimer = 0.0f;
    }
}
