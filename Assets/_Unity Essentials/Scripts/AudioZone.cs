using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioZone : MonoBehaviour
{
    [SerializeField] private AudioClip music;
    [SerializeField] private AudioSource source;

    // Start is called before the first frame update
    void Start()
    {
        if (source == null) { GetComponentInParent<AudioSource>(); }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            ChangeMusic();
        }
    }

    private void ChangeMusic()
    {
        if (source.clip == music) { return; }

        source.Stop();
        source.clip = music;
        source.Play();
    }
}
