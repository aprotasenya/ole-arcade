using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    [SerializeField] Vector3 rotationVector = new Vector3(0,0.5f,0);
    [SerializeField] GameObject onCollectVFX;
    AudioSource onCollectSFX;
    float audioLength;


    // Start is called before the first frame update
    void Start()
    {
        onCollectSFX = GetComponent<AudioSource>();
        audioLength = onCollectSFX.clip.length;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(rotationVector);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            StartCoroutine(PickUpCollectable());
            
        }
    }

    private IEnumerator PickUpCollectable()
    {
        while (true)
        {
            float waitTime = audioLength;
            onCollectSFX.Play();
            yield return new WaitForSeconds(waitTime);
            Instantiate(onCollectVFX, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }

}
