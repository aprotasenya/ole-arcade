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

    public bool IsComplete { get; private set; }

    public static event Action OnComplete;

    private bool goalInitialized = false;


    public void Init()
    {
        if (goalInitialized) return;

        if (collectAllFromScene) _quantityGoal = 0;
        IsComplete = false;

        Debug.Log($"Goal Init for {collectibleConfig.name}: {QuantityGoal}; Complete: {IsComplete}");

        goalInitialized = true;
    }


    //public void SetGoalQuantity(CollectibleConfig itemType, int value)
    //{
    //    if (itemType == collectibleConfig)
    //    {
    //        QuantityGoal = value;
    //    }
    //}

    public void AddGoalQuantity(CollectibleConfig itemType, int value)
    {
        if (!goalInitialized) Init();

        if (itemType == collectibleConfig)
        {
            QuantityGoal += value;
        }
    }

    public void RemoveGoalQuantity(CollectibleConfig itemType, int value)
    {
        if (!goalInitialized) Init();

        if (itemType == collectibleConfig)
        {
            QuantityGoal -= value;
        }
    }

    private void CheckGoalComplete()
    {
        if (QuantityGoal <= 0)
        {
            IsComplete = true;
            OnComplete?.Invoke();
        }
    }


}
