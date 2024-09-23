using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputTurner : MonoBehaviour
{
    [SerializeField] string inputAxisName = "Horizontal";
    [SerializeField] Vector3 turnAngleLimit; 
    [SerializeField] Vector3 turnAngleSpeed;
    Vector3 currentAngles;

    Vector3 initialAngles;
    Vector3 minTurnAngles;
    Vector3 maxTurnAngles;

    private void PopulateInitialAngles()
    {
        initialAngles = transform.localEulerAngles;
        minTurnAngles = initialAngles - turnAngleLimit;
        maxTurnAngles = initialAngles + turnAngleLimit;

    }

    private void Start()
    {
        PopulateInitialAngles();
    }

    void Update()
    {
        if (Input.GetAxis(inputAxisName) == 0f) { return; }

        // Adjust the maximum steering angle based on input
        Vector3 turnDirection = Input.GetAxis(inputAxisName) * turnAngleSpeed; // Assuming 'Horizontal' is your input axis for steering
        currentAngles = new Vector3(
            Mathf.Clamp(initialAngles.x + turnDirection.x, minTurnAngles.x, maxTurnAngles.x),
            Mathf.Clamp(initialAngles.y + turnDirection.y, minTurnAngles.y, maxTurnAngles.y),
            Mathf.Clamp(initialAngles.z + turnDirection.z, minTurnAngles.z, maxTurnAngles.z));

        // Apply the rotation to the wheel
        transform.localEulerAngles = currentAngles;
    }

}
