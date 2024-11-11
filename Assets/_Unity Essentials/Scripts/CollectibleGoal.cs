using UnityEngine;
using System;
using NaughtyAttributes;

[Serializable]
public class CollectibleGoal
{
    public CollectibleType collectibleType;
    public bool collectAllFromScene = true;
    [SerializeField, /*HideIf("collectAllFromScene"), AllowNesting*/] int _quantityGoal;

    public int QuantityGoal
    {
        get => _quantityGoal;
        private set
        {
            _quantityGoal = value;
            CheckGoalComplete();
        }
    }

    public bool GoalComplete { get; private set; }

    public static event Action OnComplete;


    public void Init()
    {
        if (collectAllFromScene) _quantityGoal = 0;
        GoalComplete = false;
    }


    public void SetGoalQuantity(CollectibleType itemType, int value)
    {
        if (itemType == collectibleType)
        {
            QuantityGoal = value;
        }
    }

    public void AddGoalQuantity(CollectibleType itemType, int value)
    {
        if (itemType == collectibleType)
        {
            QuantityGoal += value;
        }
    }

    public void RemoveGoalQuantity(CollectibleType itemType, int value)
    {
        if (itemType == collectibleType)
        {
            QuantityGoal -= value;
        }
    }

    private void CheckGoalComplete()
    {
        if (QuantityGoal <= 0)
        {
            GoalComplete = true;
            OnComplete?.Invoke();
        }
    }


}
