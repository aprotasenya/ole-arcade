using UnityEngine;

namespace Match3
{
    [CreateAssetMenu(fileName = "Grid Item Type", menuName = "Match3/Grid Item Type")]
    public class GridItemType : CollectibleConfig
    {
        public Sprite sprite;
        public Color color;

        public bool IsClickable = true;
        public bool IsMovable = true;
        public bool IsNearDestructible = false;

    }
}
