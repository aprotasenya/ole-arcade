using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleActiveOnKey : MonoBehaviour
{
    [SerializeField] private KeyCode toggleKey = KeyCode.H;
    [SerializeField] private GameObject[] startOn;
    [SerializeField] private GameObject[] startOff;

    bool listenToKey = true;

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

    public void ToggleActive() {
        ArrayToggleActive(startOn);
        ArrayToggleActive(startOff);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(toggleKey) && listenToKey) {
            ToggleActive();
        }
    }

    public void SetListenToKey(bool value)
    {
        listenToKey = value;
    }
}
