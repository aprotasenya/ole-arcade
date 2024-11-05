using UnityEngine;

namespace Match3
{
    [CreateAssetMenu(fileName = "SoundEffectSet", menuName = "Match3/SoundEffectSet")]
    public class SoundEffectSet : ScriptableObject
    {
        public AudioClip click;
        public AudioClip selectGem;
        public AudioClip deselectGem;
        public AudioClip match;
        public AudioClip noMatch;
        public AudioClip gemPop;
        public AudioClip gemDrop;
        public AudioClip gemCreate;

    }
}
