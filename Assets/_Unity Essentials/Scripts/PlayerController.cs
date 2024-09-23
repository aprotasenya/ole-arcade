using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls player movement and rotation.
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 3.0f; // Set player's movement speed.
    [SerializeField] private float rotationSpeed = 120.0f; // Set player's rotation speed.
    [SerializeField] private float jumpForce = 5.0f;
    [SerializeField] private AudioClip jumpSFX;

    private Rigidbody rb; // Reference to player's Rigidbody.
    private AudioSource playerSound;

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody>(); // Access player's Rigidbody.
        playerSound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            Jump();
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
        Vector3 movement = transform.forward * moveVertical * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);

        // Rotate player based on horizontal input.
        float turn = Input.GetAxis("Horizontal") * rotationSpeed * Time.fixedDeltaTime;
        if (Input.GetAxis("Vertical") < 0f) { turn = turn * -1; }
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        rb.MoveRotation(rb.rotation * turnRotation);

        
    }
}