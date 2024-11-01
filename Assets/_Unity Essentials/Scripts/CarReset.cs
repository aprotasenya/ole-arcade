using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class CarReset : MonoBehaviour
{
    [SerializeField] private KeyCode restartKey;
    [SerializeField] private float startHeight;
    [SerializeField] private bool useInitialStartHeight = false;

    [Inject] private readonly PlayerController player;
    
    private Vector3 resetRotation;

    void Start()
    {
        SetInitials();
    }

    private void SetInitials()
    {
        resetRotation = player.transform.localEulerAngles;

        if (useInitialStartHeight) startHeight = player.transform.position.y;

    }

    void Update()
    {
        if (!Input.GetKeyUp(restartKey)) { return; }

        ResetPlayerCar();
    }

    private void ResetPlayerCar()
    {
        var _currentPlayerPosition = player.transform.position;
        var _resetPosition = new Vector3(_currentPlayerPosition.x, _currentPlayerPosition.y + startHeight, _currentPlayerPosition.z);
        resetRotation.y = player.transform.rotation.eulerAngles.y;

        player.transform.SetLocalPositionAndRotation(_resetPosition, Quaternion.Euler(resetRotation));

    }
}
