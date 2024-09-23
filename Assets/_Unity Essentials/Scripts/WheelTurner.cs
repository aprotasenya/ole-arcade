using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelTurner : MonoBehaviour
{

    [SerializeField] float steeringAngleLimit = 40f; // Maximum steering angle of the wheels
    [SerializeField] float steeringSpeed = 30f;
    float currentSteerAngle = 0f; // Current steer angle of the wheel

    float initialSteerAngle;
    float minSteerAngle;
    float maxSteerAngle;

    private void PopulateInitialAngles() {
        initialSteerAngle = transform.localEulerAngles.y;
        minSteerAngle = initialSteerAngle - steeringAngleLimit;
        maxSteerAngle = initialSteerAngle + steeringAngleLimit;
    }

    private void Start()
    {
        PopulateInitialAngles();
    }

    void Update()
    {
        if (Input.GetAxis("Horizontal") == 0f) { return; }

        // Adjust the maximum steering angle based on input
        float steerDirection = Input.GetAxis("Horizontal") * steeringSpeed; // Assuming 'Horizontal' is your input axis for steering
        currentSteerAngle = Mathf.Clamp(initialSteerAngle + steerDirection, minSteerAngle, maxSteerAngle);

        // Apply the rotation to the wheel
        transform.localEulerAngles = new Vector3(0f, currentSteerAngle, 0f);
    }
}
