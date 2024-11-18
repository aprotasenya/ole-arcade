using System;
using UnityEngine;

namespace Match3
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Gem : MonoBehaviour
    {
        GemType type;

        public void SetType(GemType type)
        {
            this.type = type;
            GetComponent<SpriteRenderer>().sprite = type.sprite;
            ICollectible.RaiseOnCreated(type, 1);
        }

        public GemType GetGemType() => type;

        internal void DestroyGem()
        {
            ICollectible.RaiseOnCollected(type, 1);
            Destroy(gameObject);
        }
    }
}
