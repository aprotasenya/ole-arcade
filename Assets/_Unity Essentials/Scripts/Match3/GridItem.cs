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
        public bool IsAwaitingCompleteGrid = false;
        //public bool debugNearDestruction = true;

        //public event Action OnGridItemCollected;


        HashSet<GridItem> nearDestructibles = new();

        public void Init(GridObject<GridItem> obj, GridItemType type)
        {
            gridObject = obj;
            SetItemType(type);

        }

        public void SetItemType(GridItemType type)
        {
            this.type = type;
            GetComponent<SpriteRenderer>().sprite = type.sprite;
            IsClickable = type.IsClickable;
            IsMovable = type.IsMovable;
            IsNearDestructible = type.IsNearDestructible;
            IsAwaitingCompleteGrid = type.IsNearDestructible;

        }

        public GridItemType GetItemType() => type;

        //public void OnGridComplete()
        //{
        //    if (IsNearDestructible) SubscribeForNearDestruction();
        //}

        //private void SubscribeForNearDestruction()
        //{
        //    var grid = gridObject.grid;

        //    HashSet<Vector2Int> adjacents = grid.GetAdjacentCoordinates(gridObject.x, gridObject.y);
        //    foreach (var adj in adjacents)
        //    {
        //        var item = grid.GetObject(adj.x, adj.y).GetItem();

        //        if (item.IsClickable) nearDestructibles.Add(item);
        //    }

        //    if (nearDestructibles.Count > 0)
        //    {
        //        foreach (var item in nearDestructibles)
        //        {
        //            item.OnGridItemCollected += CollectItem;

        //            if (debugNearDestruction)
        //            {
        //                Debug.DrawLine(grid.GetWorldPositionCenter(gridObject.x, gridObject.y), grid.GetWorldPositionCenter(item.gridObject.x, item.gridObject.y), Color.white, 100f);
        //            }
        //        }
        //    }
        //}

        internal void CollectItem()
        {
            //OnGridItemCollected?.Invoke();
            ICollectible.RaiseOnCollected(type, 1);
            Destroy(gameObject);
        }

        //private void OnDestroy()
        //{
        //    if (nearDestructibles.Count > 0)
        //    {
        //        foreach (var item in nearDestructibles)
        //        {
        //            item.OnGridItemCollected -= CollectItem;
        //        }
        //    }

        //}


    }



}
