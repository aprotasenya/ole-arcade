using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarBodyRocker : MonoBehaviour
{
    [SerializeField] float rockingAngleLimit = 5f; // Maximum steering angle of the wheels
    [SerializeField] float rockingSpeed = 5f;
    float currentRockAngle = 0f; // Current steer angle of the wheel

    float initialRockAngle;
    float minRockAngle;
    float maxRockAngle;

    private void PopulateInitialAngles()
    {
        initialRockAngle = transform.localEulerAngles.y;
        minRockAngle = initialRockAngle - rockingAngleLimit;
        maxRockAngle = initialRockAngle + rockingAngleLimit;
    }

    private void Start()
    {
        PopulateInitialAngles();
    }

    void Update()
    {
        if (Input.GetAxis("Horizontal") == 0f) { return; }

        // Adjust the maximum steering angle based on input
        float steerDirection = Input.GetAxis("Horizontal") * rockingSpeed; // Assuming 'Horizontal' is your input axis for steering
        currentRockAngle = Mathf.Clamp(initialRockAngle + steerDirection, minRockAngle, maxRockAngle);

        // Apply the rotation to the wheel
        transform.localEulerAngles = new Vector3(0f, 0f, currentRockAngle);
    }

}
