using UnityEngine;
using System;
using NaughtyAttributes;

[Serializable]
public class CollectibleGoal
{
    [SerializeField] GameObject item;
    [SerializeField] public bool collectAllFromScene = true;
    [SerializeField, /*HideIf("collectAllFromScene"), AllowNesting*/] int _quantityGoal;

    public int QuantityGoal { get => _quantityGoal; }

    public ICollectible collectible;
    public ICollectible.CollectibleType collectibleType;

    public void Init()
    {
        collectible = item.GetComponent<ICollectible>();
        collectibleType = collectible.type;
        if (collectAllFromScene) _quantityGoal = 0;
        //ValidateToConsole();
    }

    public void SetGoalQuantity(ICollectible.CollectibleType itemType, int value)
    {
        if (itemType == collectible.type)
        {
            _quantityGoal = value;
        }
    }

    public void AddGoalQuantity(ICollectible.CollectibleType itemType, int value)
    {
        if (itemType == collectible.type)
        {
            _quantityGoal += value;
        }
    }

    public void RemoveGoalQuantity(ICollectible.CollectibleType itemType, int value)
    {
        if (itemType == collectible.type)
        {
            _quantityGoal -= value;
        }
    }

    void ValidateToConsole()
    {
        if (collectible != null)
        {
            Debug.Log($"{item.gameObject.name} is a collectible!");
        } else
        {
            Debug.LogWarning($"{item.gameObject.name} is NOT a collectible!");
        }
    }

}
