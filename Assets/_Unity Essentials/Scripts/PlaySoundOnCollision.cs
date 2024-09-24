using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlaySoundOnCollision : MonoBehaviour
{

    [SerializeField] private bool requirePlayer = true;
    [SerializeField] private bool soundsMayStack = false;
    [SerializeField] private float volumeRandomFactor = 0f;
    [SerializeField] private float pitchRandomFactor = 0f;
    [SerializeField] private float startInactiveTime = 0f;
    private AudioSource mySound;
    private float originalVolume;
    private float originalPitch;


    void Start()
    {
        SetInitials();
    }


    private void SetInitials()
    {
        mySound = GetComponent<AudioSource>();
        if (ShouldRandomize())
        {
            originalVolume = mySound.volume;
            originalPitch = mySound.pitch;

        }
    }

    private bool MayPlay() {
        return (startInactiveTime == 0f) || (startInactiveTime <= Time.time);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!MayPlay() || (requirePlayer && collision.gameObject.GetComponent<PlayerController2D>() == null)) { return; }

        PlaySound();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!MayPlay() || (requirePlayer && collision.gameObject.GetComponent<PlayerController>() == null)) { return; }

        PlaySound();
    }


 
    private void PlaySound()
    { 

        if (mySound.clip == null) { return; }

        if (ShouldRandomize())
        {
            RandomizeSound();
        }

        if (soundsMayStack)
        {
            mySound.Play();

        }
        else
        {
            StartCoroutine(PlayAndWait());

        }
    }


    private bool ShouldRandomize()
    {
        return volumeRandomFactor != 0f || pitchRandomFactor != 0f;
    }

    private void RandomizeSound()
    {
        mySound.volume = originalVolume + Random.Range(-volumeRandomFactor, volumeRandomFactor);
        mySound.pitch = originalPitch + Random.Range(-pitchRandomFactor, pitchRandomFactor);
    }

    private IEnumerator PlayAndWait() {
        while (true)
        {
            float waitTime = mySound.clip.length / mySound.pitch;
            mySound.Play();
            yield return new WaitForSeconds(waitTime);
            yield break;
        }
    }

}
