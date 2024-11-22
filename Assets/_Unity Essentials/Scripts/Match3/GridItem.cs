using System;
using System.Collections.Generic;
using UnityEngine;

namespace Match3
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class GridItem : MonoBehaviour
    {
        GridObject<GridItem> gridObject;

        GridItemType type;

        public bool IsClickable = true;
        public bool IsMovable = true;
        public bool IsNearDestructible = false;

        public void Init(GridObject<GridItem> obj, GridItemType type)
        {
            gridObject = obj;
            SetItemType(type);

            ICollectible.RaiseOnCreated(type, 1);

        }

        public void SetItemType(GridItemType type)
        {
            this.type = type;
            GetComponent<SpriteRenderer>().sprite = type.sprite;
            IsClickable = type.IsClickable;
            IsMovable = type.IsMovable;
            IsNearDestructible = type.IsNearDestructible;

        }

        public GridItemType GetItemType() => type;


        internal void CollectItem()
        {
            ICollectible.RaiseOnCollected(type, 1);
            Destroy(gameObject);
        }



    }



}
