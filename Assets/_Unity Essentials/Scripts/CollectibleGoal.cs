using UnityEngine;
using System;
using NaughtyAttributes;

[Serializable]
public class CollectibleGoal
{
    public CollectibleConfig collectibleConfig;
    public bool collectAllFromScene = true;
    [SerializeField, /*HideIf("collectAllFromScene"), AllowNesting*/] int _quantityGoal;

    public CollectibleGoal(CollectibleConfig collectibleConfig, bool collectAllFromScene)
    {
        this.collectibleConfig = collectibleConfig;
        this.collectAllFromScene = collectAllFromScene;
    }

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


    public void SetGoalQuantity(CollectibleConfig itemType, int value)
    {
        if (itemType == collectibleConfig)
        {
            QuantityGoal = value;
        }
    }

    public void AddGoalQuantity(CollectibleConfig itemType, int value)
    {
        if (itemType == collectibleConfig)
        {
            QuantityGoal += value;
        }
    }

    public void RemoveGoalQuantity(CollectibleConfig itemType, int value)
    {
        if (itemType == collectibleConfig)
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
