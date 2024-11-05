using UnityEngine;

namespace Match3
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] SoundEffectSet SFXSet;
        [HideInInspector] public bool canPlaySounds = true;

        AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public void PlayClick() => PlayOneShotIfCan(SFXSet.click);
        public void PlaySelect() => PlayOneShotIfCan(SFXSet.selectGem);
        public void PlayDeselect() => PlayOneShotIfCan(SFXSet.deselectGem);

        public void PlayMatch() => PlayOneShotIfCan(SFXSet.match);
        public void PlayNoMatch() => PlayOneShotIfCan(SFXSet.noMatch);

        public void PlayPop() => PlayRandomPitch(SFXSet.gemPop);
        public void PlayDrop() => PlayRandomPitch(SFXSet.gemDrop);
        public void PlayCreate() => PlayRandomPitch(SFXSet.gemCreate);

        void PlayRandomPitch (AudioClip clip)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            PlayOneShotIfCan(clip);
            audioSource.pitch = 1f;
        }

        void PlayOneShotIfCan(AudioClip clip)
        {
            if (canPlaySounds && clip != null)
            {
                audioSource.PlayOneShot(clip);
            }
        }
    }
}
