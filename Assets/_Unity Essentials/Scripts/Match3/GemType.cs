using UnityEngine;

namespace Match3
{
    [CreateAssetMenu(fileName = "GemType", menuName = "Match3/GemType")]
    public class GemType : CollectibleConfig
    {
        public Sprite sprite;
        public Color color;
    }
}
