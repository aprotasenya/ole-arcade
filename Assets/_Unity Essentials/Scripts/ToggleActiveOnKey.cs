using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleActiveOnKey : MonoBehaviour
{
    [SerializeField] private KeyCode toggleKey = KeyCode.H;
    [SerializeField] private GameObject[] startOn;
    [SerializeField] private GameObject[] startOff;


    void Start()
    {
        ArraySetActive(startOn, active: true);
        ArraySetActive(startOff, active: false);

    }

    private void ArraySetActive(GameObject[] array, bool active)
    {
        foreach (var item in array)
        {
            item.SetActive(active);
        }
    }

    private void ArrayToggleActive(GameObject[] array)
    {
        foreach (var item in array)
        {
            item.SetActive(!item.activeSelf);
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(toggleKey)) {
            ArrayToggleActive(startOn);
            ArrayToggleActive(startOff);
        }
    }
}
