using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarReset : MonoBehaviour
{

    [SerializeField] private KeyCode restartKey;
    [SerializeField] private GameObject playerCarPrefab;

    private GameObject currentPlayer;
    private Vector3 spawnRotation;
    private float spawnHeight;

    // Start is called before the first frame update
    void Start()
    {
        SetInitials();
    }

    private void SetInitials()
    {
        currentPlayer = FindObjectOfType<PlayerController>().gameObject;
        spawnRotation = currentPlayer.transform.localEulerAngles;
        spawnHeight = currentPlayer.transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (!Input.GetKeyUp(restartKey)) { return; }

        ResetPlayerCar();
    }

    private void ResetPlayerCar()
    {
        var spawnPosition = new Vector3(currentPlayer.transform.position.x, currentPlayer.transform.position.y + spawnHeight, currentPlayer.transform.position.z);
        spawnRotation.y = currentPlayer.transform.rotation.eulerAngles.y;
        Destroy(currentPlayer);
        var newPlayer = Instantiate(playerCarPrefab, spawnPosition, Quaternion.Euler(spawnRotation));
        currentPlayer = newPlayer;
    }
}
