using UnityEngine;

public class RotateAlways : MonoBehaviour
{
    [SerializeField] Vector3 rotationVector = new Vector3(0f, 0.5f, 0f);

    void Update()
    {
        Rotate();
    }

    void Rotate()
    {
        transform.Rotate(rotationVector);
    }

}



