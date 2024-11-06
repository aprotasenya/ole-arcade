using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

// Controls player movement and rotation.
public class PlayerController : MonoBehaviour
{
    //public class Factory : PlaceholderFactory<PlayerController> { }

    [SerializeField] private float speedNormal = 3.0f; // Set player's movement speed.
    [SerializeField] private float speedBoosted = 5f;
    [SerializeField] private float rotationSpeed = 120.0f; // Set player's rotation speed.
    [SerializeField] private float jumpForce = 5.0f;
    [SerializeField] private AudioClip jumpSFX;

    [SerializeField] private GameObject carModelPrefab;

    private float currentSpeed = 0f;
    private Rigidbody rb; // Reference to player's Rigidbody.
    private AudioSource playerSound;

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody>(); // Access player's Rigidbody.
        playerSound = GetComponent<AudioSource>();
        currentSpeed = speedNormal;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            currentSpeed = speedBoosted;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
        {
            currentSpeed = speedNormal;
        }
    }

    private void Jump()
    {
        playerSound.clip = jumpSFX;
        playerSound.Play();
        rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
    }


    // Handle physics-based movement and rotation.
    private void FixedUpdate()
    {
        // Move player based on vertical input.
        float moveVertical = Input.GetAxis("Vertical");
        Vector3 movement = transform.forward * moveVertical * currentSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);

        // Rotate player based on horizontal input.
        float turn = Input.GetAxis("Horizontal") * rotationSpeed * Time.fixedDeltaTime;
        if (Input.GetAxis("Vertical") < 0f) { turn *= -1; }
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        rb.MoveRotation(rb.rotation * turnRotation);
    }


    public void Initialize(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;

        GetComponent<PlayerController>().enabled = true;
        GetComponent<AudioSource>().enabled = true;

        GetComponentInChildren<Camera>().enabled = true;
        GetComponentInChildren<AudioListener>().enabled = true;

    }
}

