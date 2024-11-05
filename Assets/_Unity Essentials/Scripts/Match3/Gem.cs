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
        }

        public GemType GetGemType() => type;

        internal void DestroyGem() => Destroy(gameObject);
    }
}
