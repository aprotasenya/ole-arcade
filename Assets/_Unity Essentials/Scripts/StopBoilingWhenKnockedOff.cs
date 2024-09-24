using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopBoilingWhenKnockedOff : MonoBehaviour
{

    [SerializeField] private ParticleSystem steam;
    [SerializeField] private ParticleSystem bubbles;
    [SerializeField] private AudioSource boilingSound;
    [SerializeField] private GameObject water;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor") || collision.gameObject.CompareTag("Furniture"))
        {
            steam.Stop();
            bubbles.Stop();
            boilingSound.Stop();
            water.SetActive(false);
        }
    }
}
